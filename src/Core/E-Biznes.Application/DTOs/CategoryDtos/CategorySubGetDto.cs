namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategorySubGetDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

}
