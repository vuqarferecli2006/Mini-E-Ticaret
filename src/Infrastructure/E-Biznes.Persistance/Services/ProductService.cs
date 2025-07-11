﻿using AutoMapper;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs;
using E_Biznes.Application.DTOs.FavouriteDto;
using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.DTOs.ProducDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Domain.Entities;
using E_Biznes.Persistance.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;

namespace E_Biznes.Persistance.Services;

public class ProductService : IProductService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IFileServices _fileService;

    public ProductService(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository, IMapper mapper, IFileServices fileServices, UserManager<AppUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _productRepository = productRepository;
        _mapper = mapper;
        _fileService = fileServices;
        _userManager = userManager;
    }

    public async Task<BaseResponse<string>> CreateWithImagesAsync(ProductCreateWithImagesDto dto)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not found", false, HttpStatusCode.NotFound);
        
        var product = _mapper.Map<Product>(dto);
        product.UserId = userId;
       
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangeAsync();
        List<Image> imageList = new();
        int mainIndex = dto.MainImageIndex ?? 0; // Əgər null-dursa, default olaraq ilk şəkil main olur
        for (int i = 0; i < dto.Images.Count; i++)
        {
            var file = dto.Images[i];
            var imageUrl = await _fileService.UploadAsync(file, "product-images");

            imageList.Add(new Image
            {
                ProductId = product.Id,
                Image_Url = imageUrl,
                IsMain = i == mainIndex // yalnız seçilən index əsas şəkil olacaq
            });
        }
        if (imageList.Any())
        {
            await _productRepository.AddImagesAsync(imageList);
            await _productRepository.SaveChangeAsync();
        }

        return new("Product created successfully", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> UpdateWithImagesAsync(ProductUpdateWithImagesDto dto)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not Found", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null || product.UserId != userId)
            return new("Product not found or access denied", false, HttpStatusCode.NotFound);

        _mapper.Map(dto, product);

        if (product.ProductImages.Any())
        {
            // Fiziki faylları sil
            foreach (var oldImage in product.ProductImages.ToList())
            {
                await _fileService.DeleteFileAsync(oldImage.Image_Url);
                oldImage.IsDeleted = true; // Məhsul şəkillərini silmək əvəzinə, onları silinmiş kimi işarələyirik
                await _productRepository.UpdateImage(oldImage); // onları silinmiş kimi işarələyirik
            }

        }
        _productRepository.Update(product);
        await _productRepository.SaveChangeAsync();

        List<Image> imageList = new();

        int mainIndex = dto.MainImageIndex ?? 0; // Əgər null-dursa, default olaraq ilk şəkil əsasdır

        for (int i = 0; i < dto.Images.Count; i++)
        {
            var file = dto.Images[i];
            var imageUrl = await _fileService.UploadAsync(file, "product-images");

            imageList.Add(new Image
            {
                ProductId = product.Id,
                Image_Url = imageUrl,
                IsMain = i == mainIndex // yalnız seçilən index əsas şəkil olacaq
            });
        }

        if (imageList.Any())
        {
            await _productRepository.AddImagesAsync(imageList);
            await _productRepository.SaveChangeAsync();
        }

        return new("Product updated successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> DeleteAsync(Guid productId)
    {
        if (productId == Guid.Empty)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User Not Found", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(productId); // Burada Include ilə şəkillər yüklənməlidir
        if (product == null || product.UserId != userId)
            return new("Product not found or access denied", false, HttpStatusCode.NotFound);
        foreach (var image in product.ProductImages)
        {
            await _fileService.DeleteFileAsync(image.Image_Url);
            image.IsDeleted = true; // Məhsul şəkillərini silmək əvəzinə
            await _productRepository.UpdateImage(image); // onları silinmiş kimi işarələyirik
        }

        product.IsDeleted = true; // Məhsulu silmək əvəzinə, onu silinmiş kimi işarələyirik
        _productRepository.Update(product);

        await _productRepository.SaveChangeAsync();

        return new("Product deleted successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> AddProductDisCount(Guid productId, decimal disCount)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("Unauthorized", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return new("Product not found", false, HttpStatusCode.NotFound);

        if (product.UserId != userId)
            return new("Access denied: You are not the owner of this product", false, HttpStatusCode.Forbidden);
        // Endirim faizini təyin et
        product.DiscountPercent = disCount;

        _productRepository.Update(product);
        await _productRepository.SaveChangeAsync();

        decimal discountedPrice = product.Price * (1 - (disCount / 100m));//endirim qiymeti hesablanir
        return new($"Discount applied. New discounted price is {discountedPrice:F2} AZN", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<ProductGetDto>> CancelProductDisCount(Guid productId)
    {
        if (productId == Guid.Empty)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("Unauthorized", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return new("Product not found", false, HttpStatusCode.NotFound);

        if (product.UserId != userId)
            return new("Access denied: You are not the owner of this product", false, HttpStatusCode.Forbidden);

        if (product.DiscountPercent == 0)
            return new("This product has no discount applied", false, HttpStatusCode.BadRequest);

        product.DiscountPercent = 0;
        _productRepository.Update(product);
        await _productRepository.SaveChangeAsync();

        var productDto = _mapper.Map<ProductGetDto>(product);

        return new("Discount canceled. Product is back to original price.", productDto, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<ProductGetDto>>> GetMyProductsAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("User not found", false, HttpStatusCode.Unauthorized);

        var products = await _productRepository.GetAll(isTracking: false)
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .ToListAsync();

        var productDtos = _mapper.Map<List<ProductGetDto>>(products);

        return new(productDtos, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> DeleteImageAsync(Guid imageId)
    {
        if (imageId == Guid.Empty)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new ("Unauthorized", false, HttpStatusCode.Unauthorized);

        var image = await _productRepository.GetImageByIdAsync(imageId);
        if (image == null || image.IsDeleted)
            return new("Image not found", false, HttpStatusCode.NotFound);

        var product = await _productRepository.GetByIdAsync(image.ProductId);
        if (product == null || product.UserId != userId)
            return new("Product not found or access denied", false, HttpStatusCode.Forbidden);

        await _fileService.DeleteFileAsync(image.Image_Url);

        image.IsDeleted = true; // MSoft delete

        await _productRepository.UpdateImage(image);
        await _productRepository.SaveChangeAsync();

        return new("Image deleted successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> AddProductImageAsync(Guid productId, IFormFile file)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("Unauthorized", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null || product.UserId != userId)
            return new("Product not found or access denied", false, HttpStatusCode.NotFound);

        var imageUrl = await _fileService.UploadAsync(file, "product-images");

        var image = new Image
        {
            ProductId = product.Id,
            Image_Url = imageUrl,
            IsMain = !product.ProductImages.Any()
        };

        await _productRepository.AddImagesAsync(new List<Image> { image });

        return new("Image added successfully", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> AddProductFavouriteAsync(Guid productId)
    {
        if (productId == Guid.Empty)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("Unauthorized", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return new("Product not found", false, HttpStatusCode.NotFound);

        var alreadyFavourite = await _productRepository.IsProductFavouriteAsync(productId, userId);
        if (alreadyFavourite)
            return new("Product already in favourites", false, HttpStatusCode.BadRequest);

        var favourite = new Favourite
        {
            ProductId = productId,
            UserId = userId
        };

        await _productRepository.AddFavouriteAsync(favourite);

        return new("Product added to favourites", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> DeleteProductFavouriteAsync(Guid productId)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new BaseResponse<string>("Unauthorized", false, HttpStatusCode.Unauthorized);

        var favourite = await _productRepository.GetFavouriteAsync(productId, userId);
        if (favourite == null)
            return new BaseResponse<string>("Favourite not found", false, HttpStatusCode.NotFound);

        favourite.IsDeleted = true; // Məhsul favorilərdən silmək əvəzinə, onu silinmiş kimi işarələyirikq
        await _productRepository.UpdateFavouriteAsync(favourite);
        await _productRepository.SaveChangeAsync();

        return new BaseResponse<string>("Product removed from favourites", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<FavouriteDto>>> GetAllFavouritesAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new BaseResponse<List<FavouriteDto>>("Unauthorized", false, HttpStatusCode.Unauthorized);

        var favourites = await _productRepository.GetFavouritesByUserAsync(userId);
        if (favourites == null || !favourites.Any())
        {
            return new BaseResponse<List<FavouriteDto>>(new List<FavouriteDto>(), true, HttpStatusCode.OK);
        }

        var favouriteDtos = favourites.Select(f => new FavouriteDto
        {
            ProductId = f.ProductId,
            ProductName = f.Product.Name,
            Price = f.Product.Price,
            DiscountPercent = f.Product.DiscountPercent,
            DiscountedPrice = f.Product.DiscountPercent > 0
                ? f.Product.Price - (f.Product.Price * f.Product.DiscountPercent / 100)
                : f.Product.Price,
            MainImageUrl = f.Product.ProductImages.FirstOrDefault(i => i.IsMain)?.Image_Url,
        }).ToList();

        return new BaseResponse<List<FavouriteDto>>(favouriteDtos, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<ProductGetDto>>> GetFilteredProductsAsync(ProductFilterParams filter)
    {
        var query = _productRepository.GetAll(isTracking: false)
            .Include(p=>p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.Reviews.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.User)
            .Where(p => !p.IsDeleted);

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

        var queryWithRating = query
            .Select(p => new
            {
                Product = p,
                AverageRating = p.Reviews.Any(r => !r.IsDeleted)
                    ? p.Reviews.Where(r => !r.IsDeleted).Average(r => (int)r.Rating)
                    : 0
            });

        if (filter.MinRating.HasValue && filter.MinRating != 0)
            queryWithRating = queryWithRating.Where(x => x.AverageRating >= (int)filter.MinRating);

        if (filter.MaxRating.HasValue && filter.MaxRating != 0)
            queryWithRating = queryWithRating.Where(x => x.AverageRating <= (int)filter.MaxRating);

        // Sort
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            bool isDescending = filter.SortDirection?.ToLower() == "desc";
            switch (filter.SortBy.ToLower())
            {
                case "price":
                    queryWithRating = isDescending
                        ? queryWithRating.OrderByDescending(x => x.Product.Price)
                        : queryWithRating.OrderBy(x => x.Product.Price);
                    break;
                case "name":
                    queryWithRating = isDescending
                        ? queryWithRating.OrderByDescending(x => x.Product.Name)
                        : queryWithRating.OrderBy(x => x.Product.Name);
                    break;
                case "rating":
                    queryWithRating = isDescending
                        ? queryWithRating.OrderByDescending(x => x.AverageRating)
                        : queryWithRating.OrderBy(x => x.AverageRating);
                    break;
                case "createddate":
                    queryWithRating = isDescending
                        ? queryWithRating.OrderByDescending(x => x.Product.CreatedAt)
                        : queryWithRating.OrderBy(x => x.Product.CreatedAt);
                    break;
                default:
                    queryWithRating = queryWithRating.OrderByDescending(x => x.Product.CreatedAt);
                    break;
            }
        }
        else
        {
            queryWithRating = queryWithRating.OrderBy(x => x.Product.CreatedAt);
        }

        var products = await queryWithRating.Select(x => x.Product).ToListAsync();
        var mapped = _mapper.Map<List<ProductGetDto>>(products);
        return new BaseResponse<List<ProductGetDto>>(mapped, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<ProductGetDto>>> GetAllAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("Unauthorized", false, HttpStatusCode.Unauthorized);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new("User not found", false, HttpStatusCode.NotFound);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault()?.ToLower();

        IQueryable<Product> query = _productRepository.GetAll(isTracking: false)
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.Reviews.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.User)
             .Where(predicate: p => !p.IsDeleted);

        var products = await query.ToListAsync();
        var productDtos = _mapper.Map<List<ProductGetDto>>(products);

        return new("All products", productDtos, true, HttpStatusCode.OK);
    }


}

