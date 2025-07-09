namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategoryUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; } 
}
