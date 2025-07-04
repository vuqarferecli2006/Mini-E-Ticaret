namespace E_Biznes.Application.Shared.Settings;

public class EmailSetting
{
    public string SmtpServer { get; set; } = string.Empty;

    public int SmtpPort { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = string.Empty;

    public string SenderName { get; set; } = string.Empty;
}
