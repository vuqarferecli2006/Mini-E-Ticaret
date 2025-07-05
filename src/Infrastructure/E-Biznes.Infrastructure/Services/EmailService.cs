using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.Shared.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace E_Biznes.Infrastructure.Services;

public class EmailService:IEmailService
{
    private readonly EmailSetting _settings;

    public EmailService(IOptions<EmailSetting> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(IEnumerable<string> toEmail, string subject, string body)
    {
        using var smtp = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
        {
            Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
            EnableSsl = true
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        foreach (var email in toEmail.Distinct())
        {
            message.To.Add(email);
        }

        await smtp.SendMailAsync(message);
    }
}
