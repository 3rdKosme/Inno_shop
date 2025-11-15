using MediatR;
using Inno_Shop.UserService.Application.DTOs;

namespace Inno_Shop.UserService.Application.Users.Queries.GetUserByIdAdmin;

public sealed record GetUserByIdAdminQuery(int Id) : IRequest<UserDto>;