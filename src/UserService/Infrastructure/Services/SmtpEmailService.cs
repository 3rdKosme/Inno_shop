using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Templates;
using Inno_Shop.UserService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Runtime;

namespace Inno_Shop.UserService.Infrastructure.Services;

public class SmtpEmailService(IOptions<SmtpSettings> options) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = options.Value;
    public async Task SendAsync(string to, EmailTemplate emailTemplate, object model, CancellationToken cancellationToken = default)
    {
        var (subject, html) = EmailTemplateRenderer.Render(emailTemplate, model);

        using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        using var message = new MailMessage(_smtpSettings.FromAddress, to)
        {
            Subject = subject,
            Body = html,
            IsBodyHtml = true
        };

        await client.SendMailAsync(message, cancellationToken);
    }

}
