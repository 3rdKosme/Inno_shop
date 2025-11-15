using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Commands.LoginUser;

public record LoginCommand(string Email, string Password) : IRequest<AuthResultDto>;