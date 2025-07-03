namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategoryUpdateDto
{
    public Guid Id { get; set; }                      // Yenilənəcək kateqoriyanın Id-si
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }       // null ola bilər, əgər root category-dirsə
}