using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Inno_Shop.UserService.Infrastructure.BackgroundServices;

public class TokenCleanupService(IServiceProvider serviceProvider, IOptions<TokenCleanupPolicy> tokenCleanupPolicy) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly TokenCleanupPolicy _tokenCleanupPolicy = tokenCleanupPolicy.Value;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITokenCleanupService>();

            await service.CleanupAsync();
            await Task.Delay(TimeSpan.FromHours(-_tokenCleanupPolicy.ExecutionIntervalMinutes), cancellationToken);
        }
    }
}