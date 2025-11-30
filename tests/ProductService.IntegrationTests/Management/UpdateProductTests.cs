using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.ProductService.IntegrationTests.Management;

public class UpdateProductTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task UpdateProduct_Updates_WhenOwner()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("Original", "OriginalDesc", 1, 100.0);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-User");
        
        var request = new { product.Id, Name = "Updated", Description = "UpdatedDesc", Price = (double?)150.0 };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var db2 = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updated = await db2.Products.FindAsync(product.Id);

        Assert.Equal("Updated", updated!.Name);
        Assert.Equal("Updated", updated.Name);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("Test", "Desc", 1, 100.0);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        var request = new { product.Id, Name = "Updated" };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNotFound_WhenProductNotExists()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-User");
        
        var request = new { Id = 99999, Name = "Updated" };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsUnauthorized_WhenNotOwner()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("Test", "Desc", 1, 100.0);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-2-role-User");
        
        var request = new { product.Id, Name = "Updated" };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_UpdatesAvailability_OnValidRequest()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("Test", "Desc", 1, 100.0);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-User");
        
        var request = new { product.Id, IsAvailable = false };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var db2 = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updated = await db2.Products.FindAsync(product.Id);

        Assert.NotNull(updated);
        Assert.False(updated.IsAvailable);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequest_WhenSettingAlreadyUnavailable()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("Test", "Desc", 1, 100.0);
        product.SetUnavailable();
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-User");
        
        var request = new { product.Id, IsAvailable = false };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

