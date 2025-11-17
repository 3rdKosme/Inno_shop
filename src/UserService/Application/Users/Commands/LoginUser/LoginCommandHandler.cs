using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.LoginUser;
using Inno_Shop.UserService.Domain.Entities;
using MediatR;

public class LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, 
    IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException(ErrorMessages.IncorrectPassword);
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString());
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var token = new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(token, cancellationToken);

        return new AuthResultDto(accessToken, refreshToken);
    }
}