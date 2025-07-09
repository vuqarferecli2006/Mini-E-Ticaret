namespace E_Biznes.Application.DTOs.ReviewDtos;

public record ReviewUserGetDto
{
    public Guid ReviewId { get; set; }
    public string Content { get; set; } = null!;
    public int Rating { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductImageUrl { get; set; } = null!;
}
