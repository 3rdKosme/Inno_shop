using Inno_Shop.UserService.Application.Users.Commands.ConfirmEmail;

namespace Inno_Shop.UserService.Api.Requests.User;

public record ConfirmEmailRequest(string Token)
{
    public ConfirmEmailCommand ToCommand() => new (Token);
}
