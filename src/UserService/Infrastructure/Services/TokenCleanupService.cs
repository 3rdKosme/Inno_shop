using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Inno_Shop.UserService.Infrastructure.Services;

public class TokenCleanupService(ITokenRepository<RefreshToken> refreshTokenRepository, 
    ITokenRepository<PasswordResetToken> passwordResetTokenRepository, 
    ITokenRepository<EmailConfirmationToken> emailConfirmationTokenRepository,
    IOptions<TokenCleanupPolicy> tokenCleanupPolicy) : ITokenCleanupService
{
    public async Task CleanupAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddHours(tokenCleanupPolicy.Value.ExpirationGracePeriodHours);
        var totalDeleted = 0;
        totalDeleted += await CleanupTokensAsync(refreshTokenRepository, cutoffDate, "RefreshToken");
        totalDeleted += await CleanupTokensAsync(passwordResetTokenRepository, cutoffDate, "PasswordResetToken");
        totalDeleted += await CleanupTokensAsync(emailConfirmationTokenRepository, cutoffDate, "EmailConfirmationToken");
        //logger
    }
    
    private async Task<int> CleanupTokensAsync<T>(
        ITokenRepository<T> repository, 
        DateTime cutoffDate, 
        string tokenType) where T : BaseToken
    {
        try
        {
            var tokens = await repository.GetObsoleteTokensAsync(cutoffDate);
            
            if (!tokens.Any())
            {
                //logger.LogDebug("No obsolete {TokenType} tokens found", tokenType);
                return 0;
            }
            
            var deletedCount = 0;
            foreach (var token in tokens)
            {
                try
                {
                    await repository.DeleteAsync(token.Id);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    //logger.LogError(ex, "Failed to delete {TokenType} with ID {TokenId}", tokenType, token.Id);
                }
            }
            
            //logger.LogInformation("Deleted {DeletedCount} obsolete {TokenType} tokens", deletedCount, tokenType);
            
            return deletedCount;
        }
        catch (Exception ex)
        {
            //logger.LogError(ex, "Error during {TokenType} cleanup", tokenType);
            return 0;
        }
    }
}