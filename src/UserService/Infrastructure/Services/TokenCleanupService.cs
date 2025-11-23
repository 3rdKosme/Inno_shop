using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Inno_Shop.UserService.Infrastructure.Services;

public class TokenCleanupService(ITokenRepository<BaseToken> tokenRepository, IOptions<TokenCleanupPolicy> tokenCleanupPolicy) : ITokenCleanupService
{
    public async Task CleanupAsync()
    {
        var tokens = await tokenRepository.GetObsoleteTokensAsync(DateTime.UtcNow.AddHours(tokenCleanupPolicy.Value.ExpirationGracePeriodHours));

        foreach (var token in tokens)
        {
            await tokenRepository.DeleteAsync(token.Id);
        }
    }
}