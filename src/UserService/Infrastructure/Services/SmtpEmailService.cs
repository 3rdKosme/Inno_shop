using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Templates;
using Inno_Shop.UserService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Inno_Shop.UserService.Infrastructure.Services;

public class SmtpEmailService(IOptions<SmtpSettings> smtpSettings) : IEmailService
{
    public async Task SendAsync(string to, EmailTemplate emailTemplate, object model, CancellationToken cancellationToken = default)
    {
        var (subject, html) = EmailTemplateRenderer.Render(emailTemplate, model);

        using var client = new SmtpClient(smtpSettings.Value.Host, smtpSettings.Value.Port)
        {
            Credentials = new NetworkCredential(smtpSettings.Value.Username, smtpSettings.Value.Password),
            EnableSsl = smtpSettings.Value.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        using var message = new MailMessage(smtpSettings.Value.FromAddress, to)
        {
            Subject = subject,
            Body = html,
            IsBodyHtml = true
        };

        await client.SendMailAsync(message, cancellationToken);
    }

}
