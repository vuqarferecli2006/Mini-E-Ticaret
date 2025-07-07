namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategoryMainGetDto
{
    public Guid Id {  get; set; }

    public string? Name {  get; set; }

    public string? Description {  get; set; }

    public List<CategorySubGetDto>? SubCategories {  get; set; }
}

