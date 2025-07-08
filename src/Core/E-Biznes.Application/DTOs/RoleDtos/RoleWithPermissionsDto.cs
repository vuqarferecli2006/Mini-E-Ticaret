namespace E_Biznes.Application.DTOs.RoleDtos;

public class RoleWithPermissionsDto
{
    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
}
