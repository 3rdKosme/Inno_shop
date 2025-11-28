using FluentAssertions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.LockUser;
using Inno_Shop.UserService.Domain.Entities;
using MediatR;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class LockUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new(MockBehavior.Strict);
    private readonly Mock<IEmailService> _emailService = new(MockBehavior.Strict);
    private readonly Mock<IProductServiceClient> _productClient = new(MockBehavior.Strict);

    private LockUserCommandHandler CreateHandler()
    {
        return new LockUserCommandHandler(_userRepository.Object, _emailService.Object, _productClient.Object);
    }

    private static User CreateActiveUser()
    {
        var user = User.Create(
            "John",
            "john@test.com",
            "hash"
        );
        return user;
    }

    [Fact]
    public async Task Handle_ShouldLockUser_WhenValid()
    {
        var user = CreateActiveUser();

        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _productClient.Setup(p => p.DeactivateProductsAsync(user.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _emailService.Setup(e => e.SendAsync(
                user.Email,
                EmailTemplate.Locked,
                It.IsAny<StatusChangedModel>(),
                It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();
        var command = new LockUserCommand(user.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        user.IsLocked.Should().BeTrue();
        result.Should().Be(Unit.Value);

        _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _productClient.Verify(r => r.DeactivateProductsAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _emailService.VerifyAll();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        _userRepository.Setup(r => r.GetByIdAsync(777, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var command = new LockUserCommand(777);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessRuleValidation_WhenAlreadyLocked()
    {
        var user = CreateActiveUser();
        user.Lock();

        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();
        var command = new LockUserCommand(user.Id);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<BusinessRuleValidationException>();

        _userRepository.Verify(r => r.UpdateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _productClient.Verify(r => r.DeactivateProductsAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}