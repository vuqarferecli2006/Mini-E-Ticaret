using AzBinaTeam.Application.Abstracts.Repositories;
using E_Biznes.Domain.Entities;

namespace E_Biznes.Application.Abstract.Repositories;

public interface IOrderRepository:IRepository<Order>
{
    Task<List<Order>> GetUserOrdersAsync(string userId);
    Task<List<Order>> GetSalesForSellerAsync(string sellerId);
    Task<Order?> GetOrderDetailWithProductsAsync(Guid orderId);
}
