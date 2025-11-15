namespace Inno_Shop.UserService.Application.Abstractions;

public interface IJwtTokenService
{
    string GenerateAccessToken(int userId, string email, string role);
    string GenerateRefreshToken();
}