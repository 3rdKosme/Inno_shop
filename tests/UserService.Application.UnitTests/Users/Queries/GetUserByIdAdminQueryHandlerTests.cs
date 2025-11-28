using FluentAssertions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Users.Queries.GetUserByIdAdmin;
using Inno_Shop.UserService.Domain.Entities;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Queries;

public class GetUserByIdAdminQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();

    private GetUserByIdAdminQueryHandler CreateHandler()
    {
        return new GetUserByIdAdminQueryHandler(_userRepository.Object);
    }

    private static User CreateUser(int id = 1)
    {
        var user = User.Create("Test", "test@mail.com", "hash");
        typeof(User).GetProperty("Id")!.SetValue(user, id);
        return user;
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserExists()
    {
        var user = CreateUser(10);

        _userRepository.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetUserByIdAdminQuery(10), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        result.Email.Should().Be("test@mail.com");
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        _userRepository.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();

        await handler.Invoking(h => h.Handle(new GetUserByIdAdminQuery(5)))
            .Should().ThrowAsync<NotFoundException>();
    }
}