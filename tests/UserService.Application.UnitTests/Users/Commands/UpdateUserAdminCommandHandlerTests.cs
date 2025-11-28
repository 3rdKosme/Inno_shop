using FluentAssertions;
using Moq;
using Inno_Shop.UserService.Application.Users.Commands.UpdateUserAdmin;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class UpdateUserAdminCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IEmailService> _emailService = new();

    private UpdateUserAdminCommandHandler CreateHandler() =>
        new(_userRepository.Object, _emailService.Object);

    private static User CreateUser()
    {
        var user = User.Create("OldName", "old@mail.com", "hash");
        typeof(User).GetProperty("Id")!.SetValue(user, 1);
        return user;
    }

    [Fact]
    public async Task Handle_ShouldUpdateName_AndSendEmail_WhenNameProvided()
    {
        var user = CreateUser();

        _userRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _emailService.Setup(e => e.SendAsync(
                user.Email,
                EmailTemplate.ProfileChangedAdmin,
                It.Is<ProfileChangedModel>(m => m.Name == "NewName"),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();
        var request = new UpdateUserAdminCommand(1, "NewName");

        await handler.Handle(request, CancellationToken.None);

        user.Name.Should().Be("NewName");

        _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _emailService.VerifyAll();
    }

    [Fact]
    public async Task Handle_ShouldDoNothing_WhenNameIsNull()
    {
        var user = CreateUser();

        _userRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _emailService.Setup(e => e.SendAsync(
                user.Email,
                EmailTemplate.ProfileChangedAdmin,
                It.Is<ProfileChangedModel>(m => m.Name == user.Name),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();
        var request = new UpdateUserAdminCommand(1, null);

        await handler.Handle(request, CancellationToken.None);

        user.Name.Should().Be("OldName");

        _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _emailService.VerifyAll();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        _userRepository.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var request = new UpdateUserAdminCommand(5, "Name");

        await handler.Invoking(h => h.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.UserNotFound);

        _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _emailService.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<EmailTemplate>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
