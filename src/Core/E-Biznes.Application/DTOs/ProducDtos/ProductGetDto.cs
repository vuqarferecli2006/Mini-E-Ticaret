using E_Biznes.Application.DTOs.ImageDtos;
using E_Biznes.Application.DTOs.ReviewDtos;
using E_Biznes.Domain.Entities;

namespace E_Biznes.Application.DTOs.ProducDtos;

public record ProductGetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Condition { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public string? UserId { get; set; }
    public string? CategoryName { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountedPrice => Price * (1 - DiscountPercent / 100m);
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public List<ReviewGetDto> Reviews { get; set; } = new();
    public List<ImageDto> Images { get; set; } = new();
}
