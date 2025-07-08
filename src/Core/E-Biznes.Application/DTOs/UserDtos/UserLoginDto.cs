namespace E_Biznes.Application.DTOs.UserDtos;

public record UserLoginDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
