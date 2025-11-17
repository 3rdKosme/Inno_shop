using Inno_Shop.UserService.Application.Emails;

namespace Inno_Shop.UserService.Application.Abstractions;

public interface IEmailService
{
    public Task SendAsync(string to, EmailTemplate template, object model, CancellationToken cancellationToken = default);
}