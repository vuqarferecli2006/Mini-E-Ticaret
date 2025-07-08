namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategoryMainCreateDto
{
    public string Name {  get; set; } =null!;
    public string? Description {  get; set; }
}