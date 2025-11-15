using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.DTOs;
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


        //if (_passwordHasher.VerifyPassword(request.Password, user.PasswordHash)) throw new Exception($"req: {request.Password}, hash: {user.PasswordHash}");





        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException();
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString());
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var token = new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(token, cancellationToken);

        return new AuthResultDto(accessToken, refreshToken);
    }
}