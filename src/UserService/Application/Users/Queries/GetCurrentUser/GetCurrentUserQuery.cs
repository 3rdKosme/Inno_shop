using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery() : IRequest<UserDto>;