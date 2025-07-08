namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategorySubGetDto
{
    public Guid Id {  get; set; }
    public string Name { get; set; } = null!;
    public string? Description {  get; set; }
}
