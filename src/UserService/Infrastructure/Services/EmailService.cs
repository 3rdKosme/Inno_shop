//using RazorLight;
//using Microsoft.Extensions.Options;
//using System.Net.Mail;
//using System.Net;
//using Inno_Shop.UserService.Application.Abstractions;
//using Inno_Shop.UserService.Application.Emails;
//using Inno_Shop.UserService.Infrastructure.Options;

//namespace Inno_Shop.UserService.Infrastructure.Email;

//public class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
//{
//    private readonly EmailSettings _emailSettings = emailSettings.Value;
//    private readonly RazorLightEngine _engine = new RazorLightEngineBuilder()
//            .UseFileSystemProject(_emailSettings.TemplateRoot)
//            .UseMemoryCaching()
//            .Build();

//    public async Task SendAsync(string to, EmailTemplate template, object model, CancellationToken cancellationToken = default)
//    {
//        string templateName = $"{template}.cshtml";

//        string body = await _engine.CompileRenderAsync(templateName, model);

//        using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
//        {
//            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
//            EnableSsl = true
//        };

//        var mail = new MailMessage(_emailSettings.From, to)
//        {
//            Subject = GetSubject(template),
//            Body = body,
//            IsBodyHtml = true
//        };

//        await client.SendMailAsync(mail, cancellationToken);
//    }

//    private static string GetSubject(EmailTemplate template) => template switch
//    {
//        EmailTemplate.PasswordReset => "Reset Your Password",
//        EmailTemplate.EmailConfirmation => "Confirm Your Email",
//        EmailTemplate.ProfileChangedUser => "Your Profile Was Updated",
//        EmailTemplate.ProfileChangedAdmin => "A User Profile Was Updated",
//        EmailTemplate.Activated => "Your Account Is Activated",
//        EmailTemplate.Deactivated => "Your Account Is Deactivated",
//        EmailTemplate.Locked => "Your Account Is Locked",
//        EmailTemplate.Unlocked => "Your Account Is Unlocked",
//        _ => "Notification"
//    };
//}
