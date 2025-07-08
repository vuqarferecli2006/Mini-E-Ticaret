using AutoMapper;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Domain.Entities;
using E_Biznes.Domain.Enum;
using E_Biznes.Persistance.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace E_Biznes.Persistance.Services;

public class OrderService : IOrderService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _mailService;
    private readonly IMapper _mapper;



    public OrderService(IHttpContextAccessor contextAccessor, 
                        IOrderRepository orderRepository, 
                        IProductRepository productRepository, 
                        IMapper mapper, 
                        UserManager<AppUser> userManager, 
                        IEmailService mailService,
                        AppDbContext dbContext)
    {
        _contextAccessor = contextAccessor;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _userManager = userManager;
        _mailService = mailService;
    }

    public async Task<BaseResponse<string>> CreateOrderAsync(OrderCreateDto dto)
    {
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User .not Found", false, HttpStatusCode.Unauthorized);
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new("User not found", false, HttpStatusCode.NotFound);
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            return new("Product not found", false, HttpStatusCode.NotFound);
        if (product.Stock < dto.Quantity)
        {
            return new("Quantity must not be greater than available stock", HttpStatusCode.BadRequest);
        }
        product.Stock -= dto.Quantity;
        _productRepository.Update(product);
        var order = new Order
        {
            UserId = userId,
            TotalPrice = dto.Quantity * product.Price,
            OrderProducts = new List<OrderProduct>
            {
                new OrderProduct
                {
                    ProductId = product.Id,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                }
            }
        };
        order.Status = OrderStatus.Pending; // Yeni sifariş üçün statusu "Pending" olaraq təyin edirik

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangeAsync();

        var seller = await _userManager.FindByIdAsync(product.UserId);
        if (seller is null)
            return new("Seller not found", false, HttpStatusCode.NotFound);

        var sendEmail = await GetEmailConfirm(user, dto, order.TotalPrice);
        var sellerLink = await GetSellerOrderLink(user, product, dto.Quantity, order.TotalPrice);

        // Buyer email
        await _mailService.SendEmailAsync(new List<string> { user.Email }, "Your Order Confirmation", sendEmail);

        // Seller email - bu hissəni əlavə et
        await _mailService.SendEmailAsync(new List<string> { seller.Email }, "New Order Received",
            $"You have received a new order for your product '{product.Name}' (quantity: {dto.Quantity}).<br/>" +
            $"<a href='{sellerLink}'>Click here to view order details</a>");


        return new("Order created,Send order to email", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> CancelAndNotifyOrderAsync(Guid orderId)
    {
        var order = await _orderRepository
            .GetAll(isTracking: true)
            .Where(o => o.Id == orderId)
            .Include(o => o.User) // Alıcı
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                    .ThenInclude(p => p.User) // Satıcı (Product.User navigation)
            .FirstOrDefaultAsync();

        if (order == null)
            return new("Order not found", false, HttpStatusCode.NotFound);

        if (order.Status != OrderStatus.Pending)
            return new("Only pending orders can be canceled", false, HttpStatusCode.BadRequest);

        order.Status = OrderStatus.Cancelled;
        _orderRepository.Update(order);
        await _orderRepository.SaveChangeAsync();
        // ✅ Alıcıya email
        var buyerEmail = order.User.Email;
        var buyerSubject = "Your Order Has Been Canceled";
        var buyerBody = $"Dear {order.User.FullName},\nYour order #{order.Id} has been canceled.";

        await _mailService.SendEmailAsync(new List<string> { buyerEmail }, buyerSubject, buyerBody);

        // ✅ Satıcılara email
        var sellers = order.OrderProducts
            .Select(op => op.Product.User)
            .Where(user => user != null)
            .DistinctBy(user => user.Id) // təkrar olmasın
            .ToList();

        foreach (var seller in sellers)
        {
            var subject = "Order Containing Your Product Has Been Canceled";
            var body = $"Dear {seller.FullName},\nAn order that included your product(s) has been canceled. Order ID: #{order.Id}";

            await _mailService.SendEmailAsync(new List<string> { seller.Email }, subject, body);
        }

        return new("Order canceled and notification emails sent", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<OrderGetDto>>> GetMyOrdersAsync()
    {
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not Found", false, HttpStatusCode.Unauthorized);

        var orders = _orderRepository.GetAll(true)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .Include(o => o.User) // 🔧 Bunu əlavə etdik
            .Where(o => o.UserId == userId && o.Status!=OrderStatus.Cancelled);

        var list = _mapper.Map<List<OrderGetDto>>(await orders.ToListAsync());
        return new(list, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<OrderGetDto>>> GetMySalesAsync()
    {
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not Found", false, HttpStatusCode.Unauthorized);

        var orders = _orderRepository.GetAll(true)
            .Include(o => o.User)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.Status!=OrderStatus.Cancelled && o.OrderProducts.Any(op => op.Product != null && op.Product.UserId == userId)); // <-- Soft delete filter

        var result = _mapper.Map<List<OrderGetDto>>(await orders.ToListAsync());
        return new(result, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<OrderGetDto>> GetOrderDetailAsync(Guid orderId)
    {
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not Found", false, HttpStatusCode.Unauthorized);

        var order = await _orderRepository.GetAll(isTracking: true)
            .Include(o => o.User) // Alıcı
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.Status!=OrderStatus.Cancelled); // <-- Soft delete filter

        if (order == null || (order.UserId != userId &&
            !order.OrderProducts.Any(op => op.Product != null && op.Product.UserId == userId)))
        {
            return new("Access denied", false, HttpStatusCode.Forbidden);
        }

        var dto = _mapper.Map<OrderGetDto>(order);
        return new(dto, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<OrderEmailDetailsDto>> SendOrderEmail(
    string token,
    string userId,
    Guid productId,
    int quantity,
    decimal total)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new("User not found", HttpStatusCode.NotFound);

        // ✅ Email təsdiqlə (əgər təsdiqlənməyibsə)
        if (!user.EmailConfirmed)
        {
            var confirm = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirm.Succeeded)
                return new("Email confirmation failed", HttpStatusCode.BadRequest);
        }

        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
            return new("Product not found", HttpStatusCode.NotFound);

        // ✅ Seller-i tap
        var seller = await _userManager.FindByIdAsync(product.UserId);
        if (seller is null)
            return new("Seller not found", HttpStatusCode.NotFound);

        // ✅ Email DTO hazırla (buyer üçün)
        var dto = _mapper.Map<OrderEmailDetailsDto>((user, product, quantity, total));
        var sellerLink = await GetSellerOrderLink(user, product, quantity, total);

        // ✉️ Buyer email göndər (məsələn EmailService.SendOrderConfirmation)
        await _mailService.SendEmailAsync(new List<string> { user.Email}, "Your Order Confirmation",
            $"Thank you for your order of {product.Name}, quantity: {quantity}, total: {total} AZN.");

        // ✉️ Seller email göndər

        await _mailService.SendEmailAsync(new List<string> { seller.Email }, "New Order Received",
            $"You have received a new order for your product '{product.Name}' (quantity: {quantity}).<br/>" +
            $"{sellerLink}");

        return new("Thanks for your order!", dto, true, HttpStatusCode.OK);
    }

    private async Task<string> GetEmailConfirm(AppUser user, OrderCreateDto dto, decimal totalPrice)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var emailConfirmLink = $"https://localhost:7045/api/Orders/SendOrderEmail?" +
            $"token={HttpUtility.UrlEncode(token)}" +
            $"&userId={user.Id}" +
            $"&productId={dto.ProductId}" +
            $"&quantity={dto.Quantity}" +
            $"&total={totalPrice.ToString(CultureInfo.InvariantCulture)}"; // <-- düzəliş burada

        return emailConfirmLink;
    }

    private async Task<string> GetSellerOrderLink(AppUser buyer, Product product, int quantity, decimal total)
    {
        var token = await _userManager.GenerateUserTokenAsync(buyer, "Default", "SellerViewOrder");

        var link = $"https://localhost:7045/api/Orders/SellerOrderDetails?" +
                   $"token={HttpUtility.UrlEncode(token)}" +
                   $"&buyerId={buyer.Id}" +
                   $"&productId={product.Id}" +
                   $"&quantity={quantity}" +
                   $"&total={total.ToString(CultureInfo.InvariantCulture)}";

        return link;
    }
    public async Task<BaseResponse<OrderSellerDetailDto>> GetSellerOrderDetailsAsync(
    string token,
    string buyerId,
    Guid productId,
    int quantity,
    decimal total)
    {
        var buyer = await _userManager.FindByIdAsync(buyerId);
        if (buyer == null)
            return new("Buyer not found", false, HttpStatusCode.NotFound);

        var isValid = await _userManager.VerifyUserTokenAsync(buyer, "Default", "SellerViewOrder", token);
        if (!isValid)
            return new("Invalid or expired token", false, HttpStatusCode.BadRequest);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return new("Product not found", false, HttpStatusCode.NotFound);

        var dto = new OrderSellerDetailDto
        {
            BuyerName = $"{buyer.FullName}",
            BuyerEmail = buyer.Email,
            ProductName = product.Name,
            Quantity = quantity,
            Total = total
        };

        return new(dto, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        // Sifarişi götürürük, tracking aktiv, Buyer və Seller məlumatlarıyla birlikdə
        var order = await _orderRepository.GetAll(isTracking: true)
            .Include(o => o.User) // Buyer
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                    .ThenInclude(p => p.User) // Seller
            .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

        if (order == null)
            return new("Order not found", false, HttpStatusCode.NotFound);

        var previousStatus = order.Status;

        if (previousStatus == newStatus)
            return new("New status is the same as current status", false, HttpStatusCode.BadRequest);

        order.Status = newStatus;
        _orderRepository.Update(order);
        await _orderRepository.SaveChangeAsync();

        // Status dəyişiklik linki yaradılır
        var link = GenerateStatusChangeLink(order.Id, previousStatus, newStatus);

        // Buyer-ə email göndərilir
        var buyer = order.User;
        await _mailService.SendEmailAsync(new List<string> { buyer.Email }, "Order Status Updated",
            $"Your order #{order.Id} status has been updated from <b>{previousStatus}</b> to <b>{newStatus}</b>.<br/>" +
            $"<a href='{link}'>Click here to view status change</a>");

        // Seller-lərə email göndərilir, təkrarlanan satıcılar çıxılır
        var sellers = order.OrderProducts
            .Select(op => op.Product.User)
            .Where(u => u != null)
            .DistinctBy(u => u.Id)
            .ToList();

        foreach (var seller in sellers)
        {
            await _mailService.SendEmailAsync(new List<string> { seller.Email }, "Order Status Updated",
                $"The order containing your product(s) has been updated from <b>{previousStatus}</b> to <b>{newStatus}</b>.<br/>" +
                $"<a href='{link}'>Click here to view status change</a>");
        }

        return new("Order status updated and notifications sent", true, HttpStatusCode.OK);
    }

    public string GenerateStatusChangeHtml(Guid orderId, string oldStatus, string newStatus)
    {
        return $@"
    <html>
      <head>
        <title>Order Status Changed</title>
        <style>
          body {{ font-family: Arial, sans-serif; padding: 20px; }}
          .status-old {{ color: red; }}
          .status-new {{ color: green; }}
        </style>
      </head>
      <body>
        <h2>Order Status Changed</h2>
        <p><strong>Order ID:</strong> {orderId}</p>
        <p><strong>Previous Status:</strong> <span class='status-old'>{oldStatus}</span></p>
        <p><strong>New Status:</strong> <span class='status-new'>{newStatus}</span></p>
      </body>
    </html>";
    }

    private string GenerateStatusChangeLink(Guid orderId, OrderStatus oldStatus, OrderStatus newStatus)
    {
        return $"https://localhost:7045/api/Orders/ViewStatusChange?" +
               $"orderId={orderId}" +
               $"&oldStatus={HttpUtility.UrlEncode(oldStatus.ToString())}" +
               $"&newStatus={HttpUtility.UrlEncode(newStatus.ToString())}";
    }
}
