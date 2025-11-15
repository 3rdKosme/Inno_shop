using MediatR;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using System.ComponentModel.DataAnnotations;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Commands.AddUser;

public class AddUserCommandHandler(IUserRepository userRepository, IEmailService emailService, IPasswordHasher passwordHasher, 
    IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<AddUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    

    public async Task<AuthResultDto> Handle(AddUserCommand request, CancellationToken cancellationToken = default)
    {
        if(await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new EmailAlreadyExistsException(ErrorMessages.EmailAlreadyExists);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = User.Create(request.Name, request.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString());
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var token = new Domain.Entities.RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(token, cancellationToken);

        //await _emailService.SendAsync(user.Email, );

        return new AuthResultDto(accessToken, refreshToken);
    }
}