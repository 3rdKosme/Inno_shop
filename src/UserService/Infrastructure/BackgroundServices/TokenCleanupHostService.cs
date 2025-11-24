using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Inno_Shop.UserService.Infrastructure.BackgroundServices;

public class TokenCleanupHostService(IServiceProvider serviceProvider, IOptions<TokenCleanupPolicy> tokenCleanupPolicy) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITokenCleanupService>();
            await service.CleanupAsync();
            await Task.Delay(TimeSpan.FromMinutes(tokenCleanupPolicy.Value.ExecutionIntervalMinutes), cancellationToken);
        }
    }
}