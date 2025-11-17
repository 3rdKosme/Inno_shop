using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.SendPasswordResetCode;

public sealed record SendPasswordResetCodeCommand(string Email) : IRequest<Unit>;