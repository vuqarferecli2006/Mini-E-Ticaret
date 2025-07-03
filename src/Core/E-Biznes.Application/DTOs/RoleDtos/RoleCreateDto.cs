namespace E_Biznes.Application.DTOs.RoleDtos;

public record RoleCreateDto
{
    public string Name { get; init; } = string.Empty;

    public List<string> Permissions { get; set; }
}
