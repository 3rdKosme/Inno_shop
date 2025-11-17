using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Token) : IRequest<Unit>;