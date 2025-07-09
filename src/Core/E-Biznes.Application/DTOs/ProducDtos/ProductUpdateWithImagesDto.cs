using E_Biznes.Domain.Enum;
using Microsoft.AspNetCore.Http;

namespace E_Biznes.Application.DTOs.ProducDtos;

public record ProductUpdateWithImagesDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public ProductCondition Condition { get; set; }
    public int Stock { get; set; }
    public int? MainImageIndex { get; set; }
    public decimal? DiscountPercent { get; set; }
    public List<IFormFile> Images { get; set; } = new();
}
