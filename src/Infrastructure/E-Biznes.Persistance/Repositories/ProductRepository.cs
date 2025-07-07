using AzBinaTeam.Persistance.Repositories;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Domain.Entities;
using E_Biznes.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;

namespace E_Biznes.Persistance.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task AddImagesAsync(IEnumerable<Image> images)
    {
        await _context.Images.AddRangeAsync(images);
        await _context.SaveChangesAsync();
    }
    public async Task RemoveImagesAsync(List<Image> images)
    {
        _context.Images.RemoveRange(images);
        await _context.SaveChangesAsync();
    }
    public override async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.ProductImages)
            .Include(p=>p.Reviews)
                .ThenInclude(r=>r.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Image?> GetImageByIdAsync(Guid imageId)
    {
        return await _context.Images.FindAsync(imageId);
    }

    public void RemoveImage(Image image)
    {
        _context.Images.Remove(image);
    }

    public async Task<bool> IsProductFavouriteAsync(Guid productId, string userId)
    {
        return await _context.Favourites.AnyAsync(f => f.ProductId == productId && f.UserId == userId);
    }

    public async Task AddFavouriteAsync(Favourite favourite)
    {
        await _context.Favourites.AddAsync(favourite);
        await _context.SaveChangesAsync();
    }
    public async Task RemoveFavouriteAsync(Favourite favourite)
    {
        _context.Favourites.Remove(favourite);
        await _context.SaveChangesAsync();
    }

    public async Task<Favourite?> GetFavouriteAsync(Guid productId, string userId)
    {
        return await _context.Favourites
            .FirstOrDefaultAsync(f => f.ProductId == productId && f.UserId == userId);
    }
    public async Task<List<Favourite>> GetFavouritesByUserAsync(string userId)
    {
        return await _context.Favourites
            .Include(f => f.Product)   // Məhsul məlumatını da gətir
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }

}
