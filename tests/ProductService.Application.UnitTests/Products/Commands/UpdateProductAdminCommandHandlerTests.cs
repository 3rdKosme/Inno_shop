using FluentAssertions;
using Moq;
using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Products.Commands.UpdateProductAdmin;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Application.Common.Exceptions;

namespace Inno_Shop.ProductService.Application.UnitTests.Products.Commands
{
    public class UpdateProductAdminCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly UpdateProductAdminCommandHandler _handler;

        public UpdateProductAdminCommandHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _handler = new UpdateProductAdminCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Product_Not_Found()
        {
            var command = new UpdateProductAdminCommand(1, "NewName", "NewDesc");

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>();

            _repositoryMock.Verify(r =>
                r.GetProductByIdAsync(command.Id, It.IsAny<CancellationToken>()),
                Times.Once);

            _repositoryMock.Verify(r =>
                r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Update_Name_When_Valid()
        {
            var product = new Product("Old", "Desc", 5, 10);
            var command = new UpdateProductAdminCommand(1, "NewName", null);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            await _handler.Handle(command, CancellationToken.None);

            product.Name.Should().Be("NewName");

            _repositoryMock.Verify(r =>
                r.UpdateProductAsync(product, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Invalid_Name()
        {
            var product = new Product("Old", "Desc", 5, 10);

            var command = new UpdateProductAdminCommand(1, " ", null);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<BusinessRuleValidationException>();

            _repositoryMock.Verify(r =>
                r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Update_Description_When_Valid()
        {
            var product = new Product("Name", "OldDesc", 5, 10);
            var command = new UpdateProductAdminCommand(1, null, "NewDesc");

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            await _handler.Handle(command, CancellationToken.None);

            product.Description.Should().Be("NewDesc");

            _repositoryMock.Verify(r =>
                r.UpdateProductAsync(product, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task Handle_Should_Throw_When_Invalid_Description()
        {
            var product = new Product("Name", "OldDesc", 5, 10);

            var command = new UpdateProductAdminCommand(1, null, " ");

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<BusinessRuleValidationException>();

            _repositoryMock.Verify(r =>
                r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        
        [Fact]
        public async Task Handle_Should_Update_When_No_Fields_Provided()
        {
            var product = new Product("Name", "Desc", 5, 10);
            var command = new UpdateProductAdminCommand(1, null, null);

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            await _handler.Handle(command, CancellationToken.None);

            _repositoryMock.Verify(r =>
                r.UpdateProductAsync(product, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
