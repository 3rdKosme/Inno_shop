using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.LockUser;

public sealed record LockUserCommand(int Id) : IRequest<Unit>;