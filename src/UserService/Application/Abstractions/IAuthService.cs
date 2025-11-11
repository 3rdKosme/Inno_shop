namespace Inno_Shop.UserService.Application.Abstractions;

public interface IEmailService
{
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}