using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.SendEmailConfirmationCode;

public sealed record SendEmailConfirmationCodeCommand() : IRequest<Unit>;