using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.PromoteUserToAdmin;

public sealed record PromoteUserToAdminCommand(int Id) : IRequest<Unit>;