using MediatR;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Queries.Common;

public abstract class UserQueryHandlerBase(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDto> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        return new UserDto
        (
            Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            Role: user.UserRole.ToString(),
            IsEmailConfirmed: user.IsEmailConfirmed,
            IsActive: user.IsActive,
            CreatedAt: user.CreatedAt
        );
    }
}