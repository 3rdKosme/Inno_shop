using Inno_Shop.UserService.Application.Users.Commands.RefreshToken;

namespace Inno_Shop.UserService.Api.Requests.Auth;

public record RefreshTokenRequest(string Token)
{
    public RefreshTokenCommand ToCommand() => new RefreshTokenCommand(Token);
}