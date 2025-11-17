using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Options;

namespace Inno_Shop.UserService.Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, 
    IJwtTokenService jwtTokenService, IOptions<RefreshTokenSettings> refreshTokenSettings) : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly RefreshTokenSettings _refreshTokenSettings = refreshTokenSettings.Value;

    public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        var stored = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (stored == null || stored.IsRevoked || stored.IsExpired)
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _userRepository.GetByIdAsync(stored.UserId, cancellationToken);

        if (user == null) 
        { 
            throw new UnauthorizedAccessException();
        }

        stored.Revoke();

        await _refreshTokenRepository.UpdateAsync(stored, cancellationToken);

        var newAccess = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString());
        var newRefresh = _jwtTokenService.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new Domain.Entities.RefreshToken(user.Id, newRefresh, DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpireDays)), cancellationToken);

        return new AuthResultDto(newAccess, newRefresh);
    }
}