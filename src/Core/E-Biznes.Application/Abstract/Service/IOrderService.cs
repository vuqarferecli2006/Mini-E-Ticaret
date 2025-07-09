using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Domain.Enum;

namespace E_Biznes.Application.Abstract.Service;

public interface IOrderService
{
    Task<BaseResponse<string>> CreateOrderAsync(OrderCreateDto dto);
    Task<BaseResponse<List<OrderGetDto>>> GetMyOrdersAsync();
    Task<BaseResponse<List<OrderGetDto>>> GetMySalesAsync();
    Task<BaseResponse<OrderGetDto>> GetOrderDetailAsync(Guid orderId);
    Task<BaseResponse<OrderEmailDetailsDto>> SendOrderEmail(string token, string userId, Guid productId, int quantity, decimal total);
    Task<BaseResponse<string>> CancelAndNotifyOrderAsync(Guid orderId);
    Task<BaseResponse<OrderSellerDetailDto>> GetSellerOrderDetailsAsync(string token,string buyerId,Guid productId,int quantity,decimal total);
    string GenerateStatusChangeHtml(Guid orderId, string oldStatus, string newStatus);
    Task<BaseResponse<string>> ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus);
}
