using E_Biznes.Application.DTOs.ImageDtos;
using E_Biznes.Application.DTOs.ReviewDtos;
using E_Biznes.Domain.Entities;

namespace E_Biznes.Application.DTOs.ProducDtos;

public record ProductGetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Condition { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? UserId { get; set; }
    public string? CategoryName { get; set; }

    public List<ImageDto> Images { get; set; } = new();

    public decimal AverageRating { get; set; }
    public List<ReviewGetDto> Reviews { get; set; } = new();
}
