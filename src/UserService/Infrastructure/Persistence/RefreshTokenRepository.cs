using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;

namespace Inno_Shop.UserService.Infrastructure.Persistence;

public class RefreshTokenRepository(AppDbContext context) : TokenRepository<RefreshToken>(context), IRefreshTokenRepository { }