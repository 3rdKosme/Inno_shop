using System.Net;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.ProductService.IntegrationTests.Catalog;

public class GetAllProductsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetAllProducts_ReturnsProducts_OnSuccess()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product1 = new Product("Product1", "Desc1", 1, 100.0);
        var product2 = new Product("Product2", "Desc2", 2, 200.0);
        db.Products.Add(product1);
        db.Products.Add(product2);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/catalog");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsEmpty_WhenNoProducts()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/catalog");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProducts_FiltersByQuery_WhenQueryProvided()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("TestProduct", "Description", 1, 150.0);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/catalog?Name=TestProduct");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

