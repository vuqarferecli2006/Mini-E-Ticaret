namespace E_Biznes.Application.DTOs.UserDtos;

public record UserAddRoleDto
{
    public Guid UserId { get; set; }
    public List<Guid> RoleId { get; set; } = new();
};
