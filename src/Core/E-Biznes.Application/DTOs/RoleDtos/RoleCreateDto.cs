namespace E_Biznes.Application.DTOs.RoleDtos;

public record RoleCreateDto
{
    public string Name { get; init; } = null!;
    public List<string> Permissions { get; set; } = new();
}
