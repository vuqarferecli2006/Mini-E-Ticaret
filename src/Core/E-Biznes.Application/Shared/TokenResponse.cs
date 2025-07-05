namespace E_Biznes.Application.Shared;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpireDate { get; set; }

    public DateTime RefreshTokenExpireDate { get; set; }
}
