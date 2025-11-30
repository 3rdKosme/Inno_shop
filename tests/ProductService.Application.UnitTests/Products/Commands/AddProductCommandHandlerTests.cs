using FluentAssertions;
using Moq;
using Inno_Shop.ProductService.Application.Products.Commands.AddProduct;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.Shared.Application.Common.Constants;
using MediatR;

namespace Inno_Shop.ProductService.Application.UnitTests.Products.Commands
{
    public class AddProductCommandHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly AddProductCommandHandler _handler;

        public AddProductCommandHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _productRepositoryMock = new Mock<IProductRepository>();

            _handler = new AddProductCommandHandler(
                _currentUserServiceMock.Object,
                _productRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_Should_AddProduct_When_UserIdExists()
        {
            var command = new AddProductCommand(
                Name: "Test Product",
                Description: "Test Description",
                Price: 99.99
            );

            _currentUserServiceMock.Setup(x => x.UserId).Returns(42);

            _productRepositoryMock
                .Setup(x => x.AddProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);

            _productRepositoryMock.Verify(
                x => x.AddProductAsync(
                    It.Is<Product>(p =>
                        p.Name == command.Name &&
                        p.Description == command.Description &&
                        p.Price == command.Price &&
                        p.UserId == 42
                    ),
                    It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_Should_ThrowUnauthorized_When_UserIdIsNull()
        {
            var command = new AddProductCommand(
                Name: "Test",
                Description: "Test",
                Price: 10
            );

            _currentUserServiceMock.Setup(x => x.UserId).Returns((int?)null);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*" + ErrorMessages.UserNotFound + "*");

            _productRepositoryMock.Verify(
                x => x.AddProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}
