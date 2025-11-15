using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Commands.DeactivateUser;

public sealed record DeactivateUserCommand(string Password) : IRequest<Unit>;