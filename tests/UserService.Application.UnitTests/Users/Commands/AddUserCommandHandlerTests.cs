using FluentAssertions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.AddUser;
using Inno_Shop.UserService.Domain.Entities;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class AddUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<ITokenRepository<RefreshToken>> _refreshTokenRepositoryMock;
    private readonly AddUserCommandHandler _handler;

    public AddUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _refreshTokenRepositoryMock = new Mock<ITokenRepository<RefreshToken>>();

        _handler = new AddUserCommandHandler(
            _userRepositoryMock.Object,
            _emailServiceMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _refreshTokenRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateUserAndReturnTokens()
    {
        var command = new AddUserCommand
        (
            "John Doe",
            "john@example.com",
            "SecurePass123!"
        );

        _userRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.HashPassword(command.Password))
            .Returns("hashed_password_123");

        _jwtTokenServiceMock
            .Setup(s => s.GenerateAccessToken(It.IsAny<int>(), command.Email, It.IsAny<string>()))
            .Returns("access_token_xyz");

        _jwtTokenServiceMock
            .Setup(s => s.GenerateRefreshToken())
            .Returns("refresh_token_abc");

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _refreshTokenRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _emailServiceMock
            .Setup(e => e.SendAsync(
                command.Email,
                EmailTemplate.ProfileCreated,
                It.IsAny<ProfileCreatedModel>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access_token_xyz");
        result.RefreshToken.Should().Be("refresh_token_abc");

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _emailServiceMock.Verify(r => r.SendAsync(
            command.Email,
            EmailTemplate.ProfileCreated,
            It.Is<ProfileCreatedModel>(m => m.Name == "John Doe"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowEmailAlreadyExistsException()
    {
        var command = new AddUserCommand
        (
            "John Doe",
            "existing@example.com",
            "Pass123!"
        );

        _userRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<EmailAlreadyExistsException>()
            .WithMessage(ErrorMessages.EmailAlreadyExists);
    }
}