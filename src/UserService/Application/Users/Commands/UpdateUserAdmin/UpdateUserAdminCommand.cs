using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Commands.UpdateUserAdmin;

public sealed record UpdateUserCommand(int id, string? Name) : IRequest<Unit>;