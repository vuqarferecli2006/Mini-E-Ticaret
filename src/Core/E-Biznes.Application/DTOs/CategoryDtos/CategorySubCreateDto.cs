namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategorySubCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
}
