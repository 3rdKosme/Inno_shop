using System.Net;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.ProductService.IntegrationTests.Catalog;

public class GetProductByIdTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetProductById_ReturnsProduct_WhenExists()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("TestProduct", "TestDesc", 1, 99.99);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/catalog/{product.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFound_WhenNotExists()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/catalog/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

