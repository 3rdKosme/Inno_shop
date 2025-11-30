using Inno_Shop.UserService.Application.Users.Commands.ResetPassword;

namespace Inno_Shop.UserService.Api.Requests.Auth;

public record ResetPasswordRequest(string Token, string NewPassword)
{
    public ResetPasswordCommand ToCommand() => 
        new (Token, NewPassword);
}
