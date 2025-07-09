using E_Biznes.Domain.Enum;

namespace E_Biznes.Application.DTOs.ReviewDtos;

public record ReviewCreateDto
{
    public string Content { get; set; } = null!;
    public Rating Rating { get; set; }
}
