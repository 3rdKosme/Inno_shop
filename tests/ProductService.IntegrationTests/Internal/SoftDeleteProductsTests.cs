using System.Net;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.ProductService.IntegrationTests.Internal;

public class SoftDeleteProductsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task SoftDeleteProducts_DeletesAllUserProducts_OnValidRequest()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product1 = new Product("Product1", "Desc1", 1, 100.0);
        var product2 = new Product("Product2", "Desc2", 1, 200.0);
        var product3 = new Product("Product3", "Desc3", 2, 300.0);
        db.Products.AddRange(product1, product2, product3);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("ProductServiceKey", "test-service-key");

        var response = await client.PostAsync("/internal/users/1/deactivate", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var db2 = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

        var deleted1 = await db2.Products.FindAsync(product1.Id);
        var deleted2 = await db2.Products.FindAsync(product2.Id);
        var notDeleted = await db2.Products.FindAsync(product3.Id);

        Assert.True(deleted1.IsDeleted);
        Assert.True(deleted2.IsDeleted);
        Assert.False(notDeleted.IsDeleted);

    }

    [Fact]
    public async Task SoftDeleteProducts_ReturnsUnauthorized_WhenNoServiceKey()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsync("/internal/users/1/deactivate", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SoftDeleteProducts_ReturnsUnauthorized_WhenInvalidServiceKey()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("ProductServiceKey", "wrong-key");
        
        var response = await client.PostAsync("/internal/users/1/deactivate", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SoftDeleteProducts_HandlesEmptyProducts_WhenUserHasNoProducts()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("ProductServiceKey", "test-service-key");
        
        var response = await client.PostAsync("/internal/users/999/deactivate", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

