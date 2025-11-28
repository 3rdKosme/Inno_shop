using FluentAssertions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.UnlockUser;
using Inno_Shop.UserService.Domain.Entities;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class UnlockUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IProductServiceClient> _productClient = new();

    private UnlockUserCommandHandler CreateHandler()
    {
        return new UnlockUserCommandHandler(_userRepo.Object, _emailService.Object, _productClient.Object);
    }

    private static User CreateUser(string name, string email, string passwordHash, int id)
    {
        var user = User.Create(name, email, passwordHash);
        typeof(User).GetProperty("Id")!
            .SetValue(user, id);

        return user;
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        _userRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new UnlockUserCommand(1)));
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessRuleValidation_WhenAlreadyUnlocked()
    {
        var user = CreateUser("John", "john@test.com", "pwd", 1);

        _userRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
            handler.Handle(new UnlockUserCommand(1)));
    }

    [Fact]
    public async Task Handle_ShouldUnlockUser_UpdateRepository_RecoverProducts_AndSendEmail()
    {
        var user = CreateUser("John", "john@test.com", "pwd", 1);
        user.Lock();

        _userRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();

        await handler.Handle(new UnlockUserCommand(1));

        user.IsLocked.Should().BeFalse();

        _userRepo.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _productClient.Verify(p => p.RecoverProductsAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _emailService.Verify(e =>
                e.SendAsync(user.Email,
                    EmailTemplate.Unlocked,
                    It.Is<StatusChangedModel>(m => m.Name == "John"),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}