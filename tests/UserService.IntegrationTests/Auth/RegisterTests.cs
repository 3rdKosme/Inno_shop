using System.Net;
using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.Auth;

public class RegisterTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Register_CreatesUser_OnValidData()
    {
        var client = factory.CreateClient();
        var req = new { Name = "Test", Email = "reg@test.com", Password = "Pa$$test" };
        var res = await client.PostAsync("/api/auth/register",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsError_OnEmailExists()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Users.Add(Inno_Shop.UserService.Domain.Entities.User.Create("Exist", "z@z.com", "pass"));
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        var req = new { Name = "T", Email = "z@z.com", Password = "ValidPass123" };
        var res = await client.PostAsync("/api/auth/register",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);
    }
}