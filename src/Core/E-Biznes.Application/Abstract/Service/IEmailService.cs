namespace E_Biznes.Application.Abstract.Service;

public interface IEmailService
{
    Task SendEmailAsync(IEnumerable<string> toEmail, string subject, string body);
}
