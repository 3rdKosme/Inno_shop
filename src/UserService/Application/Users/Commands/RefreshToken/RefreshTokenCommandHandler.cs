using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Options;

namespace Inno_Shop.UserService.Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, 
    IJwtTokenService jwtTokenService, IOptions<RefreshTokenSettings> refreshTokenSettings) : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        var stored = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (stored == null || stored.IsRevoked || stored.IsExpired)
        {
            throw new UnauthorizedAccessException();
        }

        var user = await userRepository.GetByIdAsync(stored.UserId, cancellationToken) ?? throw new UnauthorizedAccessException();

        stored.Revoke();

        await refreshTokenRepository.UpdateAsync(stored, cancellationToken);

        var newAccess = jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString());
        var newRefresh = jwtTokenService.GenerateRefreshToken();

        await refreshTokenRepository.AddAsync(new Domain.Entities.RefreshToken
            (user.Id, newRefresh, DateTime.UtcNow.AddDays(refreshTokenSettings.Value.ExpireDays)), cancellationToken);

        return new AuthResultDto(newAccess, newRefresh);
    }
}