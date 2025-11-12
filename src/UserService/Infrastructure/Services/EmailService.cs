using Inno_Shop.UserService.Application.Abstractions;

namespace Inno_Shop.UserService.Infrastructure.Services;

public class EmailService : IEmailService
{
    public async Task SendAsync(string to , string subject , string body , CancellationToken cancellationToken = default)
    {
        Console.WriteLine("[MAIL]");
    }
}
