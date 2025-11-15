using MediatR;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Users.Queries.Common;

namespace Inno_Shop.UserService.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService) : UserQueryHandlerBase(userRepository), IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();

        return await GetUserByIdAsync(userId, cancellationToken);
    }
}