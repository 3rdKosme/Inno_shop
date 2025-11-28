using FluentAssertions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Users.Commands.PromoteUserToAdmin;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Domain.Enums;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class PromoteUserToAdminCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    private PromoteUserToAdminCommandHandler CreateHandler()
    {
        return new PromoteUserToAdminCommandHandler(_userRepositoryMock.Object);
    }

    private static User CreateUser()
    {
        var user = User.Create("John", "john@test.com", "hash");

        typeof(User)
            .GetProperty("Id")!
            .SetValue(user, 5);

        return user;
    }

    [Fact]
    public async Task Handle_ShouldPromoteUser_WhenValid()
    {
        var user = CreateUser();
        user.UserRole.Should().Be(UserRole.User);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();
        var command = new PromoteUserToAdminCommand(5);

        await handler.Handle(command, CancellationToken.None);

        user.UserRole.Should().Be(UserRole.Admin);

        _userRepositoryMock.Verify(r =>
            r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var command = new PromoteUserToAdminCommand(99);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessRule_WhenUserAlreadyAdmin()
    {
        var user = CreateUser();
        user.PromoteToAdmin();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();
        var command = new PromoteUserToAdminCommand(5);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<BusinessRuleValidationException>()
            .WithMessage(Domain.Common.Constants.ErrorMessages.AlreadyPromoted);
    }
}