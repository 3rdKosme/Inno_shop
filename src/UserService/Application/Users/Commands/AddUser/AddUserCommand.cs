using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Commands.AddUser;

public sealed record AddUserCommand(string Name, string Email, string Password) : IRequest<int>;