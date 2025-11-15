using Inno_Shop.UserService.Domain.Entities;

namespace Inno_Shop.UserService.Application.Abstractions;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);

    Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}