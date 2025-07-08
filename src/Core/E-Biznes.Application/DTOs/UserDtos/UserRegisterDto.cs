using E_Biznes.Domain.Enum;

namespace E_Biznes.Application.DTOs.UserDtos;

public record UserRegisterDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; }=null!;
    public string Password { get; set; }=null!;
    public string Address { get; set; } = null!;
    public int Age { get; set; }
    public MarketplaceRole RoleId { get; set; } 
}
