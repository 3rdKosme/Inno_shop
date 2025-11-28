using FluentAssertions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.LoginUser;
using Inno_Shop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<ITokenRepository<RefreshToken>> _refreshTokenRepositoryMock = new();
    private readonly IOptions<RefreshTokenSettings> _refreshSettings = Options.Create(new RefreshTokenSettings { ExpireDays = 7 });
    
    private LoginCommandHandler CreateHandler()
    {
        return new LoginCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshSettings
        );
    }

    private static User CreateUser()
    {
        var user = User.Create("test", "email@test.com", "hash");

        typeof(User)
            .GetProperty("Id")!
            .SetValue(user, 1);

        return user;
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResult_WhenValidCredentials()
    {
        var user = CreateUser();

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(h => h.VerifyPassword("123456", user.PasswordHash))
            .Returns(true);

        _jwtTokenServiceMock.Setup(s => s.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString()))
            .Returns("access-token");

        _jwtTokenServiceMock.Setup(s => s.GenerateRefreshToken())
            .Returns("refresh-token");

        var handler = CreateHandler();
        var command = new LoginCommand(user.Email, "123456");

        var result = await handler.Handle(command, CancellationToken.None);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");

        _refreshTokenRepositoryMock.Verify(r =>
                r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidCredentialsException_WhenPasswordInvalid()
    {
        var user = CreateUser();

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(h => h.VerifyPassword("wrong", user.PasswordHash))
            .Returns(false);

        var handler = CreateHandler();
        var command = new LoginCommand(user.Email, "wrong");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>()
            .WithMessage(ErrorMessages.IncorrectPassword);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidCredentialsException_WhenUserNotFound()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("no@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var command = new LoginCommand("no@test.com", "123");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>()
            .WithMessage(ErrorMessages.IncorrectPassword);
    }
}