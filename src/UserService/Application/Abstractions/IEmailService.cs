namespace Inno_Shop.UserService.Application.Abstractions;

public interface IEmailService
{
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);

    public Task SendPasswordResetLinkAsync(string to, string resetLink);

    public Task SendEmailConfirmationLinkAsync(string to, string emailConfirmationLink);
}