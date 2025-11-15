using MediatR;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Users.Queries.Common;

namespace Inno_Shop.UserService.Application.Users.Queries.GetUserByIdAdmin;

public class GetUserByIdAdminQueryHandler(IUserRepository userRepository) 
    : UserQueryHandlerBase(userRepository), IRequestHandler<GetUserByIdAdminQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdAdminQuery request, CancellationToken cancellationToken = default)
    {
        return await GetUserByIdAsync(request.Id, cancellationToken);
    }
}