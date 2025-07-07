using E_Biznes.Application.DTOs.OrderDtos;

namespace E_Biznes.Application.DTOs.UserDtos;

public record UserGetDto
{
    public string Id { get; set; }=string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<OrderGetDto> Orders { get; set; } = new();
}

