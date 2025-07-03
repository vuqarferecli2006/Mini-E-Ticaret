namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategoryMainGetDto
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public List<CategorySubGetDto>? SubCategories { get; set; }
}

