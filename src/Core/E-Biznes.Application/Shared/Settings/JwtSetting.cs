namespace E_Biznes.Application.Shared.Settings;

public class JwtSetting
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpireinMinutes { get; set; }
}
