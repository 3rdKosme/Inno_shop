using Inno_Shop.UserService.Application.DTOs;
using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResultDto>;