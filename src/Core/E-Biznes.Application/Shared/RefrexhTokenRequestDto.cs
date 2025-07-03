namespace E_Biznes.Application.Shared;

public record RefrexhTokenRequestDto
{
    public string RefreshToken { get; init; } = string.Empty;

    public string AccessToken { get; init; } = string.Empty;
}
