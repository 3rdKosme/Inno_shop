using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Commands.UpdateUserAdmin;

public sealed record UpdateUserAdminCommand(int Id, string? Name) : IRequest<Unit>;