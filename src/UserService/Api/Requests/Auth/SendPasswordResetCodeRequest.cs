using Inno_Shop.UserService.Application.Users.Commands.SendPasswordResetCode;

namespace Inno_Shop.UserService.Api.Requests.Auth;

public record SendPasswordResetCodeRequest(string Email)
{
    public SendPasswordResetCodeCommand ToCommand() =>
        new SendPasswordResetCodeCommand(Email);
}
