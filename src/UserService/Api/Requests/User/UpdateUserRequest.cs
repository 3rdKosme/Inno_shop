using Inno_Shop.UserService.Application.Users.Commands.UpdateUser;

namespace Inno_Shop.UserService.Api.Requests.User;

public record UpdateUserRequest(
    string Password,
    string? Name,
    string? Email,
    string? NewPassword)
{
    public UpdateUserCommand  ToCommand() => 
        new (Password, Name, Email, NewPassword);
}