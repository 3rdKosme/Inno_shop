using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.ProductService.IntegrationTests.Management;

public class AddProductTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task AddProduct_CreatesProduct_OnValidData()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-User");
        
        var request = new { Name = "NewProduct", Description = "Product description", Price = 99.99 };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = db.Products.FirstOrDefault(p => p.Name == "NewProduct");
        Assert.NotNull(product);
        Assert.Equal(1, product.UserId);
    }

    [Fact]
    public async Task AddProduct_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        var client = factory.CreateClient();
        var request = new { Name = "Product", Description = "Desc", Price = 50.0 };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddProduct_ReturnsBadRequest_OnInvalidName()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-User");
        
        var request = new { Name = "", Description = "Desc", Price = 50.0 };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddProduct_ReturnsBadRequest_OnInvalidPrice()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-1-role-User");
        
        var request = new { Name = "Product", Description = "Desc", Price = -10.0 };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("/api/management", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

