using AzBinaTeam.Persistance.Repositories;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Domain.Entities;
using E_Biznes.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;

namespace E_Biznes.Persistance.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetUserOrdersAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Order>> GetSalesForSellerAsync(string sellerId)
    {
        return await _context.Orders
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.OrderProducts.Any(op => op.Product.UserId == sellerId))
            .ToListAsync();
    }

    public async Task<Order?> GetOrderDetailWithProductsAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }
}
