using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Commands.ActivateUser;

public sealed record ActivateUserCommand(int Id, string Password) : IRequest<Unit>;