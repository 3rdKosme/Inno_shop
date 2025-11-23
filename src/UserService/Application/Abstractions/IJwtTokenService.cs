namespace Inno_Shop.UserService.Application.Abstractions;

public interface IJwtTokenService
{
    public string GenerateAccessToken(int userId, string email, string role);
    public string GenerateRefreshToken();
}