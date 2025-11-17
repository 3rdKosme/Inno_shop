using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.UnlockUser;

public sealed record UnlockUserCommand(int Id) : IRequest<Unit>;