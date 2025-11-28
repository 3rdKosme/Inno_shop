using FluentAssertions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Application.Users.Commands.RefreshToken;
using Inno_Shop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<ITokenRepository<RefreshToken>> _refreshTokenRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IJwtTokenService> _jwtMock = new();

    private readonly RefreshTokenSettings _settings = new()
    {
        ExpireDays = 7
    };

    private RefreshTokenCommandHandler CreateHandler()
    {
        return new RefreshTokenCommandHandler(
            _refreshTokenRepoMock.Object,
            _userRepoMock.Object,
            _jwtMock.Object,
            Options.Create(_settings)
        );
    }

    private static User CreateUser()
    {
        var user = User.Create("John", "john@test.com", "hash");

        typeof(User).GetProperty("Id")!.SetValue(user, 10);

        return user;
    }

    private static RefreshToken CreateRefreshToken(int userId)
    {
        return new RefreshToken(userId, "old-refresh", DateTime.UtcNow.AddDays(5));
    }

    [Fact]
    public async Task Handle_ShouldRefreshTokens_WhenValid()
    {
        var user = CreateUser();
        var token = CreateRefreshToken(user.Id);

        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync("old-refresh", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtMock.Setup(j => j.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString()))
            .Returns("new-access");

        _jwtMock.Setup(j => j.GenerateRefreshToken())
            .Returns("new-refresh");

        var handler = CreateHandler();
        var command = new RefreshTokenCommand("old-refresh");

        var result = await handler.Handle(command, CancellationToken.None);

        result.AccessToken.Should().Be("new-access");
        result.RefreshToken.Should().Be("new-refresh");

        token.IsRevoked.Should().BeTrue();

        _refreshTokenRepoMock.Verify(r =>
            r.UpdateAsync(token, It.IsAny<CancellationToken>()), Times.Once);

        _refreshTokenRepoMock.Verify(r =>
                r.AddAsync(It.Is<RefreshToken>(t =>
                        t.UserId == user.Id && t.Token == "new-refresh"),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenTokenNotFound()
    {
        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var handler = CreateHandler();
        var command = new RefreshTokenCommand("missing");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenTokenRevoked()
    {
        var stored = CreateRefreshToken(5);
        stored.Revoke();

        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync("old-refresh", It.IsAny<CancellationToken>()))
            .ReturnsAsync(stored);

        var handler = CreateHandler();
        var command = new RefreshTokenCommand("old-refresh");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenTokenExpired()
    {
        var stored = new RefreshToken(5, "old-refresh", DateTime.UtcNow.AddDays(-1)); // expired

        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync("old-refresh", It.IsAny<CancellationToken>()))
            .ReturnsAsync(stored);

        var handler = CreateHandler();
        var command = new RefreshTokenCommand("old-refresh");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        var stored = CreateRefreshToken(5);

        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync("old-refresh", It.IsAny<CancellationToken>()))
            .ReturnsAsync(stored);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var command = new RefreshTokenCommand("old-refresh");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}