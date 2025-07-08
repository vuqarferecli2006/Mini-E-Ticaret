namespace E_Biznes.Application.DTOs.ReviewDtos;

public record ReviewGetDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public int Rating { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? UserFullName { get; set; }
}
