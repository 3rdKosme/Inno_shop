using MediatR;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Users.Queries.Common;
using Inno_Shop.Shared.Application.Abstractions;

namespace Inno_Shop.UserService.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    : UserQueryHandlerBase(userRepository), IRequestHandler<GetCurrentUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        return await GetUserByIdAsync(userId, cancellationToken);
    }
}