using AutoMapper;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs;
using E_Biznes.Application.DTOs.ProducDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;

namespace E_Biznes.Persistance.Services;

public class ProductService : IProductService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IFileServices _fileService;

    public ProductService(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository, IMapper mapper, IFileServices fileServices)
    {
        _httpContextAccessor = httpContextAccessor;
        _productRepository = productRepository;
        _mapper = mapper;
        _fileService = fileServices;
    }
        
    public async Task<BaseResponse<string>> CreateWithImagesAsync(ProductCreateWithImagesDto dto)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return new BaseResponse<string>("User Not found", false, HttpStatusCode.NotFound);
        }

        var product = _mapper.Map<Product>(dto);
        product.UserId = userId;

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangeAsync();

        List<Image> imageList = new();
        bool isFirst = true;

        foreach (var file in dto.Images)
        {
            var imageUrl = await _fileService.UploadAsync(file, "product-images");

            var image = new Image
            {
                ProductId = product.Id,
                Image_Url = imageUrl,
                IsMain = isFirst
            };

            imageList.Add(image);
            isFirst = false;
        }

        // Add directly to DB
        await _productRepository.AddImagesAsync(imageList); // ya da context.Images.AddRangeAsync(imageList)
        await _productRepository.SaveChangeAsync();
        return new BaseResponse<string>("Product created successfully", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> UpdateWithImagesAsync(ProductUpdateWithImagesDto dto)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new BaseResponse<string>("User Not Found", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null || product.UserId != userId)
            return new BaseResponse<string>("Product not found or access denied", false, HttpStatusCode.NotFound);

        _mapper.Map(dto, product);

        if (product.ProductImages.Any())
        {
            // Fiziki faylları sil
            foreach (var oldImage in product.ProductImages.ToList())
            {
                await _fileService.DeleteFileAsync(oldImage.Image_Url);
            }

            // DB-dən köhnə şəkilləri sil
            await _productRepository.RemoveImagesAsync(product.ProductImages.ToList());
            product.ProductImages.Clear();
        }

        _productRepository.Update(product);
        await _productRepository.SaveChangeAsync();

        List<Image> imageList = new();
        bool isFirst = true;
        foreach (var file in dto.Images)
        {
            var imageUrl = await _fileService.UploadAsync(file, "product-images");
            imageList.Add(new Image
            {
                ProductId = product.Id,
                Image_Url = imageUrl,
                IsMain = isFirst
            });
            isFirst = false;
        }

        if (imageList.Any())
        {
            await _productRepository.AddImagesAsync(imageList);
            await _productRepository.SaveChangeAsync();
        }

        return new BaseResponse<string>("Product updated successfully", true, HttpStatusCode.OK);
    }
   
    public async Task<BaseResponse<string>> DeleteAsync(Guid productId)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new BaseResponse<string>("User Not Found", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(productId); // Burada Include ilə şəkillər yüklənməlidir
        if (product == null || product.UserId != userId)
            return new BaseResponse<string>("Product not found or access denied", false, HttpStatusCode.NotFound);
        foreach (var image in product.ProductImages)
        {
            await _fileService.DeleteFileAsync(image.Image_Url);
        }
        await _productRepository.RemoveImagesAsync(product.ProductImages.ToList());

        _productRepository.Delete(product);
        await _productRepository.SaveChangeAsync();

        return new BaseResponse<string>("Product deleted successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<ProductGetDto>>> GetMyProductsAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new BaseResponse<List<ProductGetDto>>("Unauthorized", false, HttpStatusCode.Unauthorized);

        var products = _productRepository.GetByFiltered(p => p.UserId == userId, new Expression<Func<Product, object>>[] { p => p.ProductImages }, isTracking: false);

        var productDtos = _mapper.Map<List<ProductGetDto>>(await products.ToListAsync());

        return new BaseResponse<List<ProductGetDto>>(productDtos, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> DeleteProductImageAsync(Guid imageId)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new BaseResponse<string>("Unauthorized", false, HttpStatusCode.Unauthorized);

        var image = await _productRepository.GetImageByIdAsync(imageId);
        if (image == null)
            return new BaseResponse<string>("Image not found", false, HttpStatusCode.NotFound);

        var product = await _productRepository.GetByIdAsync(image.ProductId);
        if (product == null || product.UserId != userId)
            return new BaseResponse<string>("Access denied", false, HttpStatusCode.Forbidden);

        await _fileService.DeleteFileAsync(image.Image_Url);
        _productRepository.RemoveImage(image);
        await _productRepository.SaveChangeAsync();

        return new BaseResponse<string>("Image deleted successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> AddProductImageAsync(Guid productId, IFormFile file)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new BaseResponse<string>("Unauthorized", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null || product.UserId != userId)
            return new BaseResponse<string>("Product not found or access denied", false, HttpStatusCode.NotFound);

        var imageUrl = await _fileService.UploadAsync(file, "product-images");

        var image = new Image
        {
            ProductId = product.Id,
            Image_Url = imageUrl,
            IsMain = !product.ProductImages.Any()
        };

        await _productRepository.AddImagesAsync(new List<Image> { image });

        return new BaseResponse<string>("Image added successfully", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> AddProductFavouriteAsync(Guid productId)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new BaseResponse<string>("Unauthorized", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return new BaseResponse<string>("Product not found", false, HttpStatusCode.NotFound);

        var alreadyFavourite = await _productRepository.IsProductFavouriteAsync(productId, userId);
        if (alreadyFavourite)
            return new BaseResponse<string>("Product already in favourites", false, HttpStatusCode.BadRequest);

        var favourite = new Favourite
        {
            ProductId = productId,
            UserId = userId
        };

        await _productRepository.AddFavouriteAsync(favourite);

        return new BaseResponse<string>("Product added to favourites", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<List<ProductGetDto>>> GetFilteredProductsAsync(ProductFilterParams filter)
    {
        var query = _productRepository.GetAll(isTracking: false)
            .Include(p => p.ProductImages)
            .AsQueryable();

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Description.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            bool isDescending = filter.SortDirection?.ToLower() == "desc";

            switch (filter.SortBy.ToLower())
            {
                case "price":
                    query = isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                    break;
                case "name":
                    query = isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                    break;
                case "createddate":
                    query = isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(p => p.CreatedAt); // fallback sort
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt); // default sort if no sortBy
        }


        var products = await query.ToListAsync();
        var mapped = _mapper.Map<List<ProductGetDto>>(products);

        return new BaseResponse<List<ProductGetDto>>(mapped, true, HttpStatusCode.OK);
    }


}

