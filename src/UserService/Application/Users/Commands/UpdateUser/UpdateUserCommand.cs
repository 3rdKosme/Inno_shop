using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(string Password, string? Name, string? Email, string? NewPassword) : IRequest<Unit>;