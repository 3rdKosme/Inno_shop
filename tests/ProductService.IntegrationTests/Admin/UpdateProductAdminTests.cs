using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.ProductService.IntegrationTests.Admin;

public class UpdateProductAdminTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task UpdateProductAdmin_Updates_WhenAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("Original", "OriginalDesc", 1, 100.0);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-Admin");
        
        var request = new { Name = "AdminUpdated", Description = "AdminDesc" };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync($"/api/admin/product/{product.Id}", content);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        using var verifyScope = factory.Services.CreateScope();
        var db2 = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updated = await db2.Products.FindAsync(product.Id);
        
        Assert.NotNull(updated);
        Assert.Equal("AdminUpdated", updated.Name);
    }

    [Fact]
    public async Task UpdateProductAdmin_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("Test", "Desc", 1, 100.0);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        var request = new { Name = "Updated" };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync($"/api/admin/product/{product.Id}", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProductAdmin_ReturnsForbidden_WhenNotAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = new Product("Test", "Desc", 1, 100.0);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-User");
        
        var request = new { Name = "Updated" };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync($"/api/admin/product/{product.Id}", content);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProductAdmin_ReturnsNotFound_WhenProductNotExists()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-Admin");
        
        var request = new { Name = "Updated" };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PutAsync("/api/admin/product/99999", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

