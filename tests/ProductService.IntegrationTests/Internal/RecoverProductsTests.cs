using System.Net;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.ProductService.IntegrationTests.Internal;

public class RecoverProductsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task RecoverProducts_RecoversAllUserProducts_OnValidRequest()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product1 = new Product("Product1", "Desc1", 1, 100.0);
        var product2 = new Product("Product2", "Desc2", 1, 200.0);
        product1.Delete();
        product2.Delete();
        db.Products.AddRange(product1, product2);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("ProductServiceKey", "test-service-key");
        
        var response = await client.PostAsync("/internal/users/1/recover", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        using var verifyScope = factory.Services.CreateScope();
        var db2 = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

        var recovered1 = await db2.Products.FindAsync(product1.Id);
        var recovered2 = await db2.Products.FindAsync(product2.Id);
        
        Assert.NotNull(recovered1);
        Assert.NotNull(recovered2);
        Assert.False(recovered1.IsDeleted);
        Assert.False(recovered2.IsDeleted);
    }

    [Fact]
    public async Task RecoverProducts_ReturnsUnauthorized_WhenNoServiceKey()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsync("/internal/users/1/recover", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RecoverProducts_ReturnsUnauthorized_WhenInvalidServiceKey()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("ProductServiceKey", "wrong-key");
        
        var response = await client.PostAsync("/internal/users/1/recover", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RecoverProducts_HandlesEmptyProducts_WhenUserHasNoProducts()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("ProductServiceKey", "test-service-key");
        
        var response = await client.PostAsync("/internal/users/999/recover", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

