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
        var refreshTokens = await refreshTokenRepository.GetObsoleteTokensAsync(DateTime.UtcNow.AddHours(tokenCleanupPolicy.Value.ExpirationGracePeriodHours));

        foreach (var token in refreshTokens)
        {
            await refreshTokenRepository.DeleteAsync(token.Id);
        }
        
        var passwordTokens = await passwordResetTokenRepository.GetObsoleteTokensAsync(DateTime.UtcNow.AddHours(tokenCleanupPolicy.Value.ExpirationGracePeriodHours));

        foreach (var token in passwordTokens)
        {
            await passwordResetTokenRepository.DeleteAsync(token.Id);
        }
        
        var emailConfirmationTokens = await emailConfirmationTokenRepository.GetObsoleteTokensAsync(DateTime.UtcNow.AddHours(tokenCleanupPolicy.Value.ExpirationGracePeriodHours));

        foreach (var token in emailConfirmationTokens)
        {
            await emailConfirmationTokenRepository.DeleteAsync(token.Id);
        }
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