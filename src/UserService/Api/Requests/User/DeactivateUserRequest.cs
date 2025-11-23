using Inno_Shop.UserService.Application.Users.Commands.DeactivateUser;

namespace Inno_Shop.UserService.Api.Requests.User;

public record DeactivateUserRequest(string Password)
{
    public DeactivateUserCommand ToCommand() => new DeactivateUserCommand(Password);
}
