using AutoMapper;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.ReviewDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Domain.Entities;
using E_Biznes.Persistance.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace E_Biznes.Persistance.Services;

public class ReviewService : IReviewService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public ReviewService(
        IHttpContextAccessor contextAccessor,
        IReviewRepository reviewRepository,
        IProductRepository productRepository,
        IMapper mapper,
        AppDbContext context)
    {
        _contextAccessor = contextAccessor;
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<BaseResponse<string>> CreateReviewAsync(Guid productId, ReviewCreateDto dto)
    {
        if (productId == Guid.Empty)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("Unauthorized", false, HttpStatusCode.Unauthorized);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
            return new("Product not found", false, HttpStatusCode.NotFound);

        var review = new Review
        {
            UserId = userId,
            ProductId = productId,
            Rating = (int)dto.Rating,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveChangeAsync();

        return new("Review created successfully", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);
        var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new("Unauthorized", false, HttpStatusCode.Unauthorized);

        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null)
            return new("Review not found", false, HttpStatusCode.NotFound);

        if (review.UserId != userId)
            return new("You are not allowed to delete this review", false, HttpStatusCode.Forbidden);

        review.IsDeleted = true;
        _reviewRepository.Update(review);
        await _reviewRepository.SaveChangeAsync();

        return new("Review deleted successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<ReviewGetDto>>> GetReviewByProductIdAsync(Guid productId)
    {
        if (productId == Guid.Empty)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);

        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
            return new("Product not found", false, HttpStatusCode.NotFound);

        var reviews = await _reviewRepository
            .GetAll(isTracking: false)
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => r.ProductId == productId&&!r.IsDeleted)
            .ToListAsync();

        var dtoList = _mapper.Map<List<ReviewGetDto>>(reviews);

        return new(dtoList, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<ReviewUserGetDto>>> GetReviewsByUserIdAsync(string userId)
    {
        if (userId is null)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return new BaseResponse<List<ReviewUserGetDto>>("User not found", false, HttpStatusCode.NotFound);

        var reviews = await _context.Reviews
            .Where(r => r.UserId == userId&&!r.IsDeleted)
            .Include(r => r.Product)
                .ThenInclude(p => p.ProductImages)
            .ToListAsync();

        var mapped = _mapper.Map<List<ReviewUserGetDto>>(reviews);

        return new BaseResponse<List<ReviewUserGetDto>>(mapped, true, HttpStatusCode.OK);
    }

}

