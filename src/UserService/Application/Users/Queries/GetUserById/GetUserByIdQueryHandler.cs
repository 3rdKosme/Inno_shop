using MediatR;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null) {
            throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        return new UserDto
        (
            Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            Role: user.UserRole.ToString(),
            IsEmailConfirmed: user.IsEmailConfirmed,
            IsActive: user.IsActive
        );
    }
}