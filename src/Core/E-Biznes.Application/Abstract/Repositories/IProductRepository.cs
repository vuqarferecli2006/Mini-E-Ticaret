using AzBinaTeam.Application.Abstracts.Repositories;
using E_Biznes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Biznes.Application.Abstract.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task AddImagesAsync(IEnumerable<Image> images);

    Task<Image?> GetImageByIdAsync(Guid imageId);

    Task UpdateImage(Image images);

    Task<bool> IsProductFavouriteAsync(Guid productId, string userId);

    Task AddFavouriteAsync(Favourite favourite);

    Task UpdateFavouriteAsync(Favourite favourite);

    Task<Favourite?> GetFavouriteAsync(Guid productId, string userId);

    Task<List<Favourite>> GetFavouritesByUserAsync(string userId);
}
