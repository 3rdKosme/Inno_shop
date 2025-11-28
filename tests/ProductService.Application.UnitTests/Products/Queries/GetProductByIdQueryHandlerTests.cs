using FluentAssertions;
using Moq;
using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Products.Queries.GetProductById;
using Inno_Shop.ProductService.Application.DTOs;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.Shared.Application.Exceptions;

namespace Inno_Shop.ProductService.Application.UnitTests.Products.Queries;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _handler = new GetProductByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_ProductDto_When_Product_Exists()
    {
        var product = new Product("Name", "Desc", 5, 100);
        var command = new GetProductByIdQuery(product.Id);

        _repositoryMock
            .Setup(r => r.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Name.Should().Be(product.Name);
        result.Description.Should().Be(product.Description);
        result.Price.Should().Be(product.Price);
        result.UserId.Should().Be(product.UserId);
        result.CreatedAt.Should().Be(product.CreatedAt);
    }
    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Product_Not_Found()
    {
        var command = new GetProductByIdQuery(99);

        _repositoryMock
            .Setup(r => r.GetProductByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<NotFoundException>();
    }
}
