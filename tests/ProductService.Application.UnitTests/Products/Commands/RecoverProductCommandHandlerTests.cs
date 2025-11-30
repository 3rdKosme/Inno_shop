using FluentAssertions;
using Moq;
using MediatR;
using Inno_Shop.ProductService.Application.Products.Commands.RecoverProduct;
using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Common.Exceptions;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Domain.Common.Exceptions;

namespace Inno_Shop.ProductService.Application.UnitTests.Products.Commands
{
    public class RecoverProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly RecoverProductCommandHandler _handler;

        public RecoverProductCommandHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _handler = new RecoverProductCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Recover_Products_And_Update_Them()
        {
            var command = new RecoverProductCommand(Id: 10);

            var deletedProduct1 = new Product("A", "A Desc", 1, 100);
            deletedProduct1.Delete();

            var deletedProduct2 = new Product("B", "B Desc", 1, 200);
            deletedProduct2.Delete();

            var list = new List<Product> { deletedProduct1, deletedProduct2 };

            _repositoryMock
                .Setup(r => r.GetAllByUserIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            _repositoryMock
                .Setup(r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            var result = await _handler.Handle(command, CancellationToken.None);
            
            result.Should().Be(Unit.Value);

            deletedProduct1.IsDeleted.Should().BeFalse();
            deletedProduct2.IsDeleted.Should().BeFalse();

            _repositoryMock.Verify(
                r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2)
            );
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Product_Already_Recovered()
        {
            var command = new RecoverProductCommand(Id: 5);

            var product = new Product("Test", "Desc", 1, 50); 

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
            var command = new RecoverProductCommand(Id: 99);

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
