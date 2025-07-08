namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategorySubUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? NewParentCategoryId { get; set; }
}
