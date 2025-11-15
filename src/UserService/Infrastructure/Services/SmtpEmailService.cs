using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Inno_Shop.UserService.Infrastructure.Services;

public class SmtpEmailService(IOptions<SmtpSettings> options) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = options.Value;
    public async Task SendAsync(string to , string subject , string body, CancellationToken cancellationToken = default)
    {
        using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.FromAddress, _smtpSettings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        using (cancellationToken.Register(() => smtpClient.Dispose()))
        {
            await smtpClient.SendMailAsync(mailMessage);
        }

    }
}
