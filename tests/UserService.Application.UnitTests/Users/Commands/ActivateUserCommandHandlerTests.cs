using FluentAssertions;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.ActivateUser;
using Inno_Shop.UserService.Domain.Entities;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class ActivateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IProductServiceClient> _productServiceClient = new();

    private ActivateUserCommandHandler CreateHandler()
    {
        return new ActivateUserCommandHandler(
            _userRepository.Object,
            _emailService.Object,
            _passwordHasher.Object,
            _currentUserService.Object,
            _productServiceClient.Object);
    }

    private static User CreateInactiveUser()
    {
        var user = User.Create(
            "John",
            "john@example.com",
            "hash123");
        user.Deactivate();
        return user;
    }


    [Fact]
    public async Task Handle_WithValidRequest_ShouldActivateUserAndSendEmail()
    {
        var command = new ActivateUserCommand("123");
        var user = CreateInactiveUser();

        _currentUserService.Setup(x => x.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);

        _productServiceClient
            .Setup(s => s.RecoverProductsAsync(user.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        await handler.Handle(command, CancellationToken.None);

        user.IsActive.Should().BeTrue();

        _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _productServiceClient.Verify(r => r.RecoverProductsAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);

        _emailService.Verify(e => e.SendAsync(
                user.Email,
                EmailTemplate.Activated,
                It.Is<StatusChangedModel>(m => m.Name == user.Name),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIdIsNull_ShouldThrowUnauthorizedAccessException()
    {
        var handler = CreateHandler();
        _currentUserService.Setup(x => x.UserId).Returns((int?)null);

        var action = () => handler.Handle(new ActivateUserCommand("123"), CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        _currentUserService.Setup(x => x.UserId).Returns(12);

        _userRepository.Setup(r => r.GetByIdAsync(12, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();

        var action = () => handler.Handle(new ActivateUserCommand("123"), CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_WithIncorrectPassword_ShouldThrowInvalidCredentialsException()
    {
        var user = CreateInactiveUser();
        _currentUserService.Setup(x => x.UserId).Returns(user.Id);

        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), user.PasswordHash))
            .Returns(false);

        var handler = CreateHandler();

        var action = () => handler.Handle(
            new ActivateUserCommand("x"), CancellationToken.None);

        await action.Should()
            .ThrowAsync<InvalidCredentialsException>()
            .WithMessage(ErrorMessages.IncorrectPassword);
    }

    [Fact]
    public async Task Handle_WhenAlreadyActive_ShouldThrowBusinessRuleValidationException()
    {
        var user = CreateInactiveUser();
        user.Activate();

        _currentUserService.Setup(x => x.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), user.PasswordHash))
            .Returns(true);

        var handler = CreateHandler();

        var action = () => handler.Handle(
            new ActivateUserCommand("123"), CancellationToken.None);

        await action.Should()
            .ThrowAsync<BusinessRuleValidationException>();
    }
}