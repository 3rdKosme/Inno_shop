using FluentAssertions;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Users.Queries.GetCurrentUser;
using Inno_Shop.UserService.Domain.Entities;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Queries;

public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();

    private GetCurrentUserQueryHandler CreateHandler()
    {
        return new GetCurrentUserQueryHandler(_userRepository.Object, _currentUser.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserExists()
    {
        var user = User.Create("John", "john@mail.com", "hash");
        typeof(User).GetProperty("Id")!.SetValue(user, 10);

        _currentUser.Setup(c => c.UserId).Returns(10);

        _userRepository.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        result.Email.Should().Be("john@mail.com");
        result.Name.Should().Be("John");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIdIsNull()
    {
        _currentUser.Setup(c => c.UserId).Returns((int?)null);

        var handler = CreateHandler();

        await handler.Invoking(h => h.Handle(new GetCurrentUserQuery()))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        _currentUser.Setup(c => c.UserId).Returns(15);

        _userRepository.Setup(r => r.GetByIdAsync(15, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();

        await handler.Invoking(h => h.Handle(new GetCurrentUserQuery()))
            .Should().ThrowAsync<NotFoundException>();
    }
}