using Inno_Shop.UserService.Application.Users.Commands.LoginUser;

namespace Inno_Shop.UserService.Api.Requests.Auth;

public record LoginRequest(string Email, string Password)
{
    public LoginCommand ToCommand() => new (Email, Password);
}