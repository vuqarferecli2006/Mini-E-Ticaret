using AzBinaTeam.Application.Abstracts.Repositories;
using E_Biznes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Biznes.Application.Abstract.Repositories;

public interface IProductRepository : IRepository<Product>
{
    public Task AddImagesAsync(IEnumerable<Image> images);

    public Task RemoveImagesAsync(List<Image> images);

    // ProductRepository əlavə metodlar

    public Task<Image?> GetImageByIdAsync(Guid imageId);

    public void RemoveImage(Image image);

    public Task<bool> IsProductFavouriteAsync(Guid productId, string userId);

    public Task AddFavouriteAsync(Favourite favourite);
}
