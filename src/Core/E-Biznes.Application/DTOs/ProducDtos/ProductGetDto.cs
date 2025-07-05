using E_Biznes.Application.DTOs.ImageDtos;

namespace E_Biznes.Application.DTOs.ProducDtos;

public class ProductGetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Condition { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? UserId { get; set; }

    public List<ImageDto> Images { get; set; } = new();
}
