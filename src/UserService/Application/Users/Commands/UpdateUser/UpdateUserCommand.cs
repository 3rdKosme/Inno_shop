using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Commands.AddUser;

public sealed record UpdateUserCommand(string Name, string Email, string Password) : IRequest<int>;