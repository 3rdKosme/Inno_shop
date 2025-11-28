using FluentAssertions;
using Moq;
using Inno_Shop.ProductService.Application.Products.Queries.GetProducts;
using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Products.Common;
using Inno_Shop.ProductService.Domain.Entities;

namespace Inno_Shop.ProductService.Application.UnitTests.Products.Queries;

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnMappedProductDtos()
    {
        var repositoryMock = new Mock<IProductRepository>();

        var products = new List<Product>
        {
            new (
                name: "Phone",
                description: "Smartphone",
                price: 999,
                userId: 5
            ),
            new (
                name: "Laptop",
                description: "Gaming laptop",
                price: 1999,
                userId: 5
            )
        };
        products[1].SetUnavailable();

        repositoryMock
            .Setup(r => r.GetProductsAsync(
                It.IsAny<ProductFilter>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new GetProductsQueryHandler(repositoryMock.Object);

        var query = new GetProductsQuery(
            Search: null,
            MinPrice: null,
            MaxPrice: null,
            IsAvailable: null,
            UserId: null,
            Sort: null,
            Page: 1,
            PageSize: 10
        );
        
        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        result.Should().HaveCount(2);
        
        result[0].Name.Should().Be("Phone");
        result[0].Price.Should().Be(999);
        result[0].IsAvailable.Should().BeTrue();
        
        result[1].Name.Should().Be("Laptop");
        result[1].Price.Should().Be(1999);
        result[1].IsAvailable.Should().BeFalse();

        repositoryMock.Verify(r => r.GetProductsAsync(
            It.IsAny<ProductFilter>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
