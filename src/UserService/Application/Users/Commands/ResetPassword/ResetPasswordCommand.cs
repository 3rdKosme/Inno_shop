using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Unit>;