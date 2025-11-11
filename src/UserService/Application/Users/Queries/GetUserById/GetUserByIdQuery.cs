using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(int Id) : IRequest<UserDto>;