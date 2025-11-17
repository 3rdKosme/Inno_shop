using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Inno_Shop.UserService.Application.Services;

public class TokenCleanupService(ITokenRepository<BaseToken> tokenRepository, IOptions<TokenCleanupPolicy> tokenCleanupPolicy) : ITokenCleanupService
{
    private readonly ITokenRepository<BaseToken> _tokenRepository = tokenRepository;
    private readonly TokenCleanupPolicy _tokenCleanupPolicy = tokenCleanupPolicy.Value;

    public async Task CleanupAsync()
    {
        var tokens = await _tokenRepository.GetObsoleteTokensAsync(DateTime.UtcNow.AddHours(_tokenCleanupPolicy.ExpirationGracePeriodHours));

        foreach (var token in tokens)
        {
            await _tokenRepository.DeleteAsync(token.Id);
        }
    }
}