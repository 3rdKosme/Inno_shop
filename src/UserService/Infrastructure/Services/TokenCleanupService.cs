using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Inno_Shop.UserService.Infrastructure.Services;

public class TokenCleanupService(
    ITokenRepository<RefreshToken> refreshTokenRepository,
    ITokenRepository<PasswordResetToken> passwordResetTokenRepository,
    ITokenRepository<EmailConfirmationToken> emailConfirmationTokenRepository,
    IOptions<TokenCleanupPolicy> tokenCleanupPolicy) : ITokenCleanupService
{
    public async Task CleanupAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddHours(tokenCleanupPolicy.Value.ExpirationGracePeriodHours);

        await CleanupTokensAsync(refreshTokenRepository, cutoffDate, "RefreshToken");
        await CleanupTokensAsync(passwordResetTokenRepository, cutoffDate, "PasswordResetToken");
        await CleanupTokensAsync(emailConfirmationTokenRepository, cutoffDate, "EmailConfirmationToken");
    }

    private async Task<int> CleanupTokensAsync<T>(
        ITokenRepository<T> repository,
        DateTime cutoffDate,
        string tokenType) where T : BaseToken
    {
        var tokens = await repository.GetObsoleteTokensAsync(cutoffDate);

        if (!tokens.Any()) return 0;

        var deletedCount = 0;
        foreach (var token in tokens)
        {
            await repository.DeleteAsync(token.Id);
            deletedCount++;
        }

        return deletedCount;
    }
}