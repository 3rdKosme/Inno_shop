using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.DTOs;
using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

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

        await _refreshTokenRepository.RevokeAsync(stored, cancellationToken);

        var newAccess = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString());
        var newRefresh = _jwtTokenService.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new Domain.Entities.RefreshToken(user.Id, newRefresh, DateTime.UtcNow.AddDays(7)), cancellationToken);

        return new AuthResultDto(newAccess, newRefresh);
    }
}