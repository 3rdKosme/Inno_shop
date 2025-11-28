using FluentAssertions;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.UpdateUser;
using Inno_Shop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();
    private readonly Mock<ITokenRepository<RefreshToken>> _refreshTokenRepo = new();

    private readonly IOptions<RefreshTokenSettings> _refreshOpts =
        Options.Create(new RefreshTokenSettings { ExpireDays = 7 });

    private UpdateUserCommandHandler CreateHandler()
    {
        return new UpdateUserCommandHandler(
            _userRepository.Object,
            _emailService.Object,
            _passwordHasher.Object,
            _currentUser.Object,
            _jwtTokenService.Object,
            _refreshOpts,
            _refreshTokenRepo.Object);
    }

    private static User CreateUser()
    {
        var user = User.Create("OldName", "old@mail.com", "hash123");
        typeof(User).GetProperty("Id")!.SetValue(user, 1);
        return user;
    }

    [Fact]
    public async Task Handle_ShouldUpdateAllAndReturnTokens_WhenAllProvided()
    {
        var user = CreateUser();

        _currentUser.Setup(c => c.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword("current", user.PasswordHash)).Returns(true);
        _passwordHasher.Setup(h => h.HashPassword("newpass")).Returns("hashed-new");

        _userRepository.Setup(r => r.ExistsByEmailAsync("new@mail.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _jwtTokenService.Setup(j => j.GenerateAccessToken(user.Id, "new@mail.com", user.UserRole.ToString()))
            .Returns("access-token");
        _jwtTokenService.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");

        _refreshTokenRepo.Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _emailService.Setup(e => e.SendAsync("new@mail.com", EmailTemplate.ProfileChangedUser,
                It.Is<ProfileChangedModel>(m => m.Name == "NewName"), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        var request = new UpdateUserCommand(
            Name: "NewName",
            Email: "new@mail.com",
            Password: "current",
            NewPassword: "newpass");

        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");

        user.Name.Should().Be("NewName");
        user.Email.Should().Be("new@mail.com");
        user.PasswordHash.Should().Be("hashed-new");

        _refreshTokenRepo.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _emailService.VerifyAll();
    }

    [Fact]
    public async Task Handle_ShouldUpdateNameOnly_WhenEmailAndNewPasswordNull()
    {
        var user = CreateUser();

        _currentUser.Setup(c => c.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword("current", user.PasswordHash)).Returns(true);

        _userRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _emailService.Setup(e => e.SendAsync(user.Email, EmailTemplate.ProfileChangedUser,
                It.Is<ProfileChangedModel>(m => m.Name == "NewOnly"), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        var request = new UpdateUserCommand(Name: "NewOnly", Email: null, Password: "current", NewPassword: null);

        var result = await handler.Handle(request, CancellationToken.None);

        result.AccessToken.Should().BeNull();
        result.RefreshToken.Should().BeNull();
        user.Name.Should().Be("NewOnly");

        _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _emailService.VerifyAll();
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenCurrentUserIdIsNull()
    {
        _currentUser.Setup(c => c.UserId).Returns((int?)null);

        var handler = CreateHandler();
        var request = new UpdateUserCommand(Name: "x", Email: null, Password: "p", NewPassword: null);

        await handler.Invoking(h => h.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        _currentUser.Setup(c => c.UserId).Returns(1);
        _userRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var request = new UpdateUserCommand(Name: "x", Email: null, Password: "p", NewPassword: null);

        await handler.Invoking(h => h.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidCredentials_WhenPasswordIncorrect()
    {
        var user = CreateUser();

        _currentUser.Setup(c => c.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword("wrong", user.PasswordHash)).Returns(false);

        var handler = CreateHandler();
        var request = new UpdateUserCommand(Name: "x", Email: null, Password: "wrong", NewPassword: null);

        await handler.Invoking(h => h.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<InvalidCredentialsException>()
            .WithMessage(ErrorMessages.IncorrectPassword);

        _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldConvertNameDomainException_ToBusinessRule()
    {
        var user = CreateUser();

        _currentUser.Setup(c => c.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword("current", user.PasswordHash)).Returns(true);

        var handler = CreateHandler();
        var request = new UpdateUserCommand(Name: "", Email: null, Password: "current", NewPassword: null);

        await handler.Invoking(h => h.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<BusinessRuleValidationException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowEmailAlreadyExists_WhenEmailTaken()
    {
        var user = CreateUser();

        _currentUser.Setup(c => c.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword("current", user.PasswordHash)).Returns(true);

        _userRepository.Setup(r => r.ExistsByEmailAsync("taken@mail.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = CreateHandler();
        var request =
            new UpdateUserCommand(Name: null, Email: "taken@mail.com", Password: "current", NewPassword: null);

        await handler.Invoking(h => h.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<EmailAlreadyExistsException>()
            .WithMessage(ErrorMessages.EmailAlreadyExists);

        _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldConvertEmailDomainException_ToBusinessRule()
    {
        var user = CreateUser();

        _currentUser.Setup(c => c.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword("current", user.PasswordHash)).Returns(true);

        _userRepository.Setup(r => r.ExistsByEmailAsync("", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();
        var request = new UpdateUserCommand(Name: null, Email: "", Password: "current", NewPassword: null);

        await handler.Invoking(h => h.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<BusinessRuleValidationException>();
    }

    [Fact]
    public async Task Handle_ShouldConvertNewPasswordDomainException_ToBusinessRule()
    {
        var user = CreateUser();

        _currentUser.Setup(c => c.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword("current", user.PasswordHash)).Returns(true);

        _userRepository.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasher.Setup(h => h.HashPassword("bad")).Returns(string.Empty);

        var handler = CreateHandler();
        var request = new UpdateUserCommand(Name: null, Email: null, Password: "current", NewPassword: "bad");

        await handler.Invoking(h => h.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<BusinessRuleValidationException>();

        _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}