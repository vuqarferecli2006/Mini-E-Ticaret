using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.Shared;

namespace E_Biznes.Application.Abstract.Service;

public interface IOrderService
{
    Task<BaseResponse<string>> CreateOrderAsync(CreateOrderDto dto);
    Task<BaseResponse<List<OrderGetDto>>> GetMyOrdersAsync();
    Task<BaseResponse<List<OrderGetDto>>> GetMySalesAsync();
    Task<BaseResponse<OrderGetDto>> GetOrderDetailAsync(Guid orderId);
    Task<BaseResponse<OrderEmailDetailsDto>> SendOrderEmail(string token, string userId, Guid productId, int quantity, decimal total);
}
