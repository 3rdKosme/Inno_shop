using FluentAssertions;
using Moq;
using MediatR;
using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Common.Exceptions;
using Inno_Shop.ProductService.Application.Products.Commands.SoftDeleteProduct;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Domain.Common.Exceptions;

namespace Inno_Shop.ProductService.Application.UnitTests.Products.Commands
{
    public class SoftDeleteProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly SoftDeleteProductCommandHandler _handler;

        public SoftDeleteProductCommandHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _handler = new SoftDeleteProductCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Delete_Products_And_Update_Them()
        {
            var command = new SoftDeleteProductCommand(Id: 10);

            var p1 = new Product("A", "A desc", 1, 100);
            var p2 = new Product("B", "B desc", 1, 200);

            var list = new List<Product> { p1, p2 };

            _repositoryMock
                .Setup(r => r.GetAllByUserIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            _repositoryMock
                .Setup(r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);

            p1.IsDeleted.Should().BeTrue();
            p2.IsDeleted.Should().BeTrue();

            _repositoryMock.Verify(
                r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2)
            );
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Product_Already_Deleted()
        {
            var command = new SoftDeleteProductCommand(Id: 5);

            var product = new Product("Test", "Desc", 1, 50);
            product.Delete();

            _repositoryMock
                .Setup(r => r.GetAllByUserIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<BusinessRuleValidationException>();

            _repositoryMock.Verify(
                r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_Should_DoNothing_When_NoProductsFound()
        {
            var command = new SoftDeleteProductCommand(Id: 123);

            _repositoryMock
                .Setup(r => r.GetAllByUserIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());
            
            var result = await _handler.Handle(command, CancellationToken.None);
            
            result.Should().Be(Unit.Value);

            _repositoryMock.Verify(
                r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}
