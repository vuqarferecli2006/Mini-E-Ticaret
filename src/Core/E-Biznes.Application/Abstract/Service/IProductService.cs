using E_Biznes.Application.DTOs;
using E_Biznes.Application.DTOs.FavouriteDto;
using E_Biznes.Application.DTOs.ProducDtos;
using E_Biznes.Application.Shared;
using Microsoft.AspNetCore.Http;

namespace E_Biznes.Application.Abstract.Service;

public interface IProductService
{
    Task<BaseResponse<string>> CreateWithImagesAsync(ProductCreateWithImagesDto dto);
    Task<BaseResponse<string>> UpdateWithImagesAsync(ProductUpdateWithImagesDto dto);
    Task<BaseResponse<string>> DeleteAsync(Guid productId);
    Task<BaseResponse<List<ProductGetDto>>> GetMyProductsAsync();
    Task<BaseResponse<string>> DeleteImageAsync(Guid imageId);
    Task<BaseResponse<string>> AddProductImageAsync(Guid productId, IFormFile file);
    Task<BaseResponse<string>> AddProductFavouriteAsync(Guid productId);
    Task<BaseResponse<List<ProductGetDto>>> GetFilteredProductsAsync(ProductFilterParams filter);
    Task<BaseResponse<string>> DeleteProductFavouriteAsync(Guid productId);
    Task<BaseResponse<List<FavouriteDto>>> GetAllFavouritesAsync();
    Task<BaseResponse<List<ProductGetDto>>> GetAllAsync();
    Task<BaseResponse<string>> AddProductDisCount(Guid productId, decimal disCount);
    Task<BaseResponse<ProductGetDto>> CancelProductDisCount(Guid productId);
}
