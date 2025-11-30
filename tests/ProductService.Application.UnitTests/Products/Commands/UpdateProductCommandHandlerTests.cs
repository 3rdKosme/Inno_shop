using FluentAssertions;
using Moq;
using MediatR;
using Inno_Shop.ProductService.Application.Products.Commands.UpdateProduct;
using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Application.Common.Exceptions;
using Inno_Shop.Shared.Application.Exceptions;

namespace Inno_Shop.ProductService.Application.UnitTests.Products.Commands
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _currentUserMock = new Mock<ICurrentUserService>();
            _handler = new UpdateProductCommandHandler(_currentUserMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_UserId_IsNull()
        {
            _currentUserMock.Setup(x => x.UserId).Returns((int?)null);

            var cmd = new UpdateProductCommand(Id: 1, Name: "X", Description: null, Price: null, IsAvailable: null);
            
            Func<Task> act = async () => await _handler.Handle(cmd, CancellationToken.None);
            
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Product_NotFound()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(1);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var cmd = new UpdateProductCommand(Id: 10, Name: "X", null, null, null);
            
            Func<Task> act = async () => await _handler.Handle(cmd, CancellationToken.None);
            
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Product_Does_Not_Belong_To_User()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(1);

            var product = new Product("P", "D", userId: 99, price: 10);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var cmd = new UpdateProductCommand(5, "New", null, null, null);

            Func<Task> act = async () => await _handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_Should_Update_Fields_And_Save()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(1);

            var product = new Product("Old", "OldDesc", 1, 100);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var cmd = new UpdateProductCommand(
                Id: 1,
                Name: "NewName",
                Description: "NewDesc",
                IsAvailable: null,
                Price: 150
            );

            var result = await _handler.Handle(cmd, CancellationToken.None);

            result.Should().Be(Unit.Value);

            product.Name.Should().Be("NewName");
            product.Description.Should().Be("NewDesc");
            product.Price.Should().Be(150);
            product.IsAvailable.Should().BeTrue();

            _repositoryMock.Verify(
                r => r.UpdateProductAsync(product, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_Should_Throw_BusinessRule_When_Name_Invalid()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(1);

            var product = new Product("Old", "OldDesc", 1, 10);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var cmd = new UpdateProductCommand(1, Name: "", null, null, null);

            Func<Task> act = async () => await _handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleValidationException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_BusinessRule_When_Description_Invalid()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(1);

            var product = new Product("Old", "OldDesc", 1, 10);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var cmd = new UpdateProductCommand(1, null, Description: "", null, null);

            Func<Task> act = async () => await _handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleValidationException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_BusinessRule_When_IsAvailable_InvalidState()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(1);

            var product = new Product("P", "D", 1, 10);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var cmd = new UpdateProductCommand(1, null, null, true, null);

            Func<Task> act = async () => await _handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleValidationException>();
        }

        [Fact]
        public async Task Handle_Should_Allow_IsAvailable_To_Set_False()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(1);

            var product = new Product("P", "D", 1, 10);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var cmd = new UpdateProductCommand(1, null, null, false, null);

            var result = await _handler.Handle(cmd, CancellationToken.None);

            result.Should().Be(Unit.Value);
            product.IsAvailable.Should().BeFalse();
        }
    }
}
