using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.ResetPassword;
using Inno_Shop.UserService.Domain.Entities;
using MediatR;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ITokenRepository<PasswordResetToken>> _tokenRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    private ResetPasswordCommandHandler CreateHandler()
    {
        return new ResetPasswordCommandHandler(_userRepo.Object, _tokenRepo.Object, _hasher.Object);
    }

    [Fact]
    public async Task Handle_Should_ResetPassword_When_Token_Valid()
    {
        var token = new PasswordResetToken(1, "valid-token", DateTime.UtcNow.AddHours(1));

        var user = User.Create("user", "mail@test.com", "old");
        typeof(User)
            .GetProperty("Id")!
            .SetValue(user, 1);

        _tokenRepo.Setup(r => r.GetByTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _userRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _hasher.Setup(h => h.HashPassword("newpass")).Returns("hashed");

        var handler = CreateHandler();

        var result = await handler.Handle(new ResetPasswordCommand("valid-token", "newpass"));

        Assert.Equal(Unit.Value, result);
        Assert.True(token.IsRevoked);
        Assert.Equal("hashed", user.PasswordHash);

        _userRepo.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _tokenRepo.Verify(r => r.UpdateAsync(token, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Token_Not_Found()
    {
        _tokenRepo.Setup(r => r.GetByTokenAsync("123", It.IsAny<CancellationToken>()))
            .ReturnsAsync((PasswordResetToken?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            handler.Handle(new ResetPasswordCommand("123", "pass")));
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Token_Expired()
    {
        var token = new PasswordResetToken(1, "t", DateTime.UtcNow.AddHours(-1));

        _tokenRepo.Setup(r => r.GetByTokenAsync("t", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<TokenIsExpiredOrRevokedException>(() =>
            handler.Handle(new ResetPasswordCommand("t", "pass")));
    }

    [Fact]
    public async Task Handle_Should_Throw_When_User_Not_Found()
    {
        var token = new PasswordResetToken(1, "t", DateTime.UtcNow.AddHours(1));

        _tokenRepo.Setup(r => r.GetByTokenAsync("t", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _userRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new ResetPasswordCommand("t", "pass")));
    }

    [Fact]
    public async Task Handle_Should_Convert_DomainArgumentNullException_To_BusinessRuleException()
    {
        var token = new PasswordResetToken(1, "t", DateTime.UtcNow.AddHours(1));
        var user = User.Create("user", "mail@test.com", "old");
        typeof(User)
            .GetProperty("Id")!
            .SetValue(user, 1);

        _tokenRepo.Setup(r => r.GetByTokenAsync("t", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        _userRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _hasher.Setup(h => h.HashPassword("x123123123")).Returns("");

        var handler = CreateHandler();

        await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
            handler.Handle(new ResetPasswordCommand("t", "x123123123")));
    }
}