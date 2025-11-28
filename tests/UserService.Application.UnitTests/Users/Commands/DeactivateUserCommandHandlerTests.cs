using FluentAssertions;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.DeactivateUser;
using Inno_Shop.UserService.Domain.Entities;
using MediatR;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class DeactivateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new(MockBehavior.Strict);
    private readonly Mock<IEmailService> _emailService = new(MockBehavior.Strict);
    private readonly Mock<IPasswordHasher> _passwordHasher = new(MockBehavior.Strict);
    private readonly Mock<ICurrentUserService> _currentUser = new(MockBehavior.Strict);
    private readonly Mock<IProductServiceClient> _productClient = new(MockBehavior.Strict);

    private DeactivateUserCommandHandler CreateHandler()
    {
        return new DeactivateUserCommandHandler(_userRepository.Object, _emailService.Object, _passwordHasher.Object,
            _currentUser.Object, _productClient.Object);
    }

    private static User CreateActiveUser()
    {
        return User.Create(
            "Test User",
            "user@test.com",
            "hashed");
    }

    [Fact]
    public async Task Handle_ShouldDeactivateUser_WhenPasswordIsCorrect()
    {
        var user = CreateActiveUser();

        _currentUser.Setup(x => x.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(p => p.VerifyPassword("pass", user.PasswordHash)).Returns(true);

        _userRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _productClient.Setup(p => p.DeactivateProductsAsync(user.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _emailService.Setup(e => e.SendAsync(
                user.Email,
                EmailTemplate.Deactivated,
                It.IsAny<StatusChangedModel>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();
        var command = new DeactivateUserCommand("pass");

        var result = await handler.Handle(command, CancellationToken.None);

        user.IsActive.Should().BeFalse();
        result.Should().Be(Unit.Value);

        _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _productClient.Verify(r => r.DeactivateProductsAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _emailService.VerifyAll();
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidCredentials_WhenPasswordIncorrect()
    {
        var user = CreateActiveUser();

        _currentUser.Setup(x => x.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(p => p.VerifyPassword("pass", user.PasswordHash)).Returns(false);

        var handler = CreateHandler();
        var command = new DeactivateUserCommand("pass");

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>()
            .WithMessage(ErrorMessages.IncorrectPassword);

        _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _productClient.Verify(p => p.DeactivateProductsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessRuleValidation_WhenAlreadyDeactivated()
    {
        var user = CreateActiveUser();
        user.Deactivate();

        _currentUser.Setup(x => x.UserId).Returns(user.Id);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(p => p.VerifyPassword("pass", user.PasswordHash)).Returns(true);

        var handler = CreateHandler();
        var command = new DeactivateUserCommand("pass");

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleValidationException>();

        _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _productClient.Verify(p => p.DeactivateProductsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        _currentUser.Setup(x => x.UserId).Returns(1);
        _userRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var command = new DeactivateUserCommand("pass");

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.UserNotFound);
    }
}