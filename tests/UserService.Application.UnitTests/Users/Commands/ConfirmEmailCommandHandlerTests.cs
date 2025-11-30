using FluentAssertions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.ConfirmEmail;
using Inno_Shop.UserService.Domain.Entities;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ITokenRepository<EmailConfirmationToken>> _tokenRepository = new();

    private ConfirmEmailCommandHandler CreateHandler()
    {
        return new ConfirmEmailCommandHandler(_userRepository.Object, _tokenRepository.Object);
    }

    private static User CreateUser()
    {
        return User.Create(
            "John",
            "john@example.com",
            "hash123");
    }

    private static EmailConfirmationToken CreateToken(int userId, bool expired = false, bool revoked = false)
    {
        var token = new EmailConfirmationToken(
            userId,
            "abc123",
            expired ? DateTime.UtcNow.AddMinutes(-5) : DateTime.UtcNow.AddMinutes(10));
        if (revoked) token.Revoke();
        return token;
    }

    [Fact]
    public async Task Handle_WithValidToken_ShouldConfirmEmailAndRevokeToken()
    {
        var command = new ConfirmEmailCommand("abc123");
        var user = CreateUser();
        var token = CreateToken(user.Id);

        _tokenRepository.Setup(r => r.GetByTokenAsync("abc123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();

        await handler.Handle(command, CancellationToken.None);

        user.IsEmailConfirmed.Should().BeTrue();
        token.IsRevoked.Should().BeTrue();

        _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _tokenRepository.Verify(r => r.UpdateAsync(token, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTokenDoesNotExist_ShouldThrowInvalidCredentialsException()
    {
        _tokenRepository.Setup(r => r.GetByTokenAsync("bad", It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmailConfirmationToken?)null);

        var handler = CreateHandler();
        var command = new ConfirmEmailCommand("bad");

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should()
            .ThrowAsync<InvalidCredentialsException>()
            .WithMessage(ErrorMessages.IncorrectToken);
    }

    [Fact]
    public async Task Handle_WhenTokenExpired_ShouldThrowTokenIsExpiredOrRevokedException()
    {
        var expiredToken = CreateToken(1, true);

        _tokenRepository.Setup(r => r.GetByTokenAsync(expiredToken.Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredToken);

        var handler = CreateHandler();
        var command = new ConfirmEmailCommand(expiredToken.Token);

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should()
            .ThrowAsync<TokenIsExpiredOrRevokedException>()
            .WithMessage(ErrorMessages.TokenIsExpiredOrRevoked);
    }

    [Fact]
    public async Task Handle_WhenTokenRevoked_ShouldThrowTokenIsExpiredOrRevokedException()
    {
        var revokedToken = CreateToken(1, revoked: true);

        _tokenRepository.Setup(r => r.GetByTokenAsync(revokedToken.Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(revokedToken);

        var handler = CreateHandler();
        var command = new ConfirmEmailCommand(revokedToken.Token);

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should()
            .ThrowAsync<TokenIsExpiredOrRevokedException>()
            .WithMessage(ErrorMessages.TokenIsExpiredOrRevoked);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        var token = CreateToken(99);

        _tokenRepository.Setup(r => r.GetByTokenAsync("abc", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _userRepository.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var command = new ConfirmEmailCommand("abc");

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyConfirmed_ShouldThrowBusinessRuleValidationException()
    {
        var user = CreateUser();
        user.ConfirmEmail();

        var token = CreateToken(user.Id);

        _tokenRepository.Setup(r => r.GetByTokenAsync(token.Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();
        var command = new ConfirmEmailCommand(token.Token);

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should()
            .ThrowAsync<BusinessRuleValidationException>();
    }
}