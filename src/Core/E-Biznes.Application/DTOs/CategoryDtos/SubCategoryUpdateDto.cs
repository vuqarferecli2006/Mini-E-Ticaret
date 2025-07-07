namespace E_Biznes.Application.DTOs.CategoryDtos;

public record SubCategoryUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid NewParentCategoryId { get; set; }
}
