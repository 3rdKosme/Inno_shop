using Inno_Shop.UserService.Application.Users.Commands.ActivateUser;

namespace Inno_Shop.UserService.Api.Requests.User;

public record ActivateUserRequest(string Password)
{
    public ActivateUserCommand ToCommand() =>
        new ActivateUserCommand(Password);
}
