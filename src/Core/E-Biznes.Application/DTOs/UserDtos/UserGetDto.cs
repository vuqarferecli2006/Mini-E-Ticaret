using E_Biznes.Application.DTOs.OrderDtos;

namespace E_Biznes.Application.DTOs.UserDtos;

public record UserGetDto
{
    public string Id { get; set; }=null!;
    public string Email { get; set; } = null!;
    public int Age { get; set; }
    public string UserName { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public List<OrderGetDto> Orders { get; set; } = new();
}

