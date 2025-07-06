using AutoMapper;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
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
                        IEmailService mailService)
    {
        _contextAccessor = contextAccessor;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _userManager = userManager;
        _mailService = mailService;
    }

    public async Task<BaseResponse<string>> CreateOrderAsync(CreateOrderDto dto)
    {
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not Found", false, HttpStatusCode.Unauthorized);
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new("User not found", false, HttpStatusCode.NotFound);
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            return new("Product not found", false, HttpStatusCode.NotFound);
        if (product.Stock < dto.Quantity)
        {
            return new("Quentity must't be than stock", HttpStatusCode.BadRequest);
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

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangeAsync();

        var sendEmail = await GetEmailConfirm(user,dto,order.TotalPrice);

        await _mailService.SendEmailAsync(new List<string> { user.Email}, "Your Order Confirmation", sendEmail);

        return new("Order created,Send order to email", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<List<OrderGetDto>>> GetMyOrdersAsync()
    {
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not Found", false, HttpStatusCode.Unauthorized);

        var orders = _orderRepository.GetAll(true)
                        .Include(o => o.OrderProducts)
                        .ThenInclude(op => op.Product)
                        .Where(o => o.UserId == userId);

        var list = _mapper.Map<List<OrderGetDto>>(await orders.ToListAsync());
        return new(list, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<OrderGetDto>>> GetMySalesAsync()
    {
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not Found", false, HttpStatusCode.Unauthorized);

        var orders = _orderRepository.GetAll(true)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.OrderProducts.Any(op => op.Product != null && op.Product.UserId == userId));

        var result = _mapper.Map<List<OrderGetDto>>(await orders.ToListAsync());
        return new(result, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<OrderGetDto>> GetOrderDetailAsync(Guid orderId)
    {
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not Found", false, HttpStatusCode.Unauthorized);

        var order = await _orderRepository.GetAll(isTracking: true)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

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

        var confirm = await _userManager.ConfirmEmailAsync(user, token);
        if (!confirm.Succeeded)
            return new("Email confirmation failed", HttpStatusCode.BadRequest);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
            return new("Product not found", HttpStatusCode.NotFound);

        var dto = _mapper.Map<OrderEmailDetailsDto>((user, product, quantity, total));

        return new("Thanks for your order!",dto,true,HttpStatusCode.OK);
    }

    private async Task<string> GetEmailConfirm(AppUser user, CreateOrderDto dto, decimal totalPrice)
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

}
