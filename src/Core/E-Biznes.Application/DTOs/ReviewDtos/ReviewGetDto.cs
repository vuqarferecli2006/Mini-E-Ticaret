namespace E_Biznes.Application.DTOs.ReviewDtos;

public record ReviewGetDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? UserFullName { get; set; }


}
