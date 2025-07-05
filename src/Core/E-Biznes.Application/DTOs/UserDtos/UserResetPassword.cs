namespace AzBinaTeam.Application.DTOs.UserDtos;

public class UserResetPasswordDto
{
    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public string NewPassword { get; set; } = null!;
}
