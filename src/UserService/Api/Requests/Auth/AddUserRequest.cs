using Inno_Shop.UserService.Application.Users.Commands.AddUser;

namespace Inno_Shop.UserService.Api.Requests.Auth;

public record AddUserRequest(string Name, string Email, string Password)
{
    public AddUserCommand ToCommand() => new (Name, Email, Password);
}
