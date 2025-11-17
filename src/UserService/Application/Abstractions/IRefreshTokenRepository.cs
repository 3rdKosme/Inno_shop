using Inno_Shop.UserService.Domain.Entities;

namespace Inno_Shop.UserService.Application.Abstractions;

public interface IRefreshTokenRepository : ITokenRepository<RefreshToken> { }