using System.Net;
using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.Auth;

public class RefreshTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Refresh_ReturnsTokens_OnValidRequest()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("Name1", "r@t.com", "pw1");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var refreshToken = new RefreshToken(user.Id, "real-refresh-token", DateTime.UtcNow.AddDays(1));
        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        var req = new { Token = "real-refresh-token" };
        var res = await client.PostAsync("/api/auth/refresh",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Refresh_Returns401_OnInvalidToken()
    {
        var client = factory.CreateClient();
        var req = new { Token = "invalidtoken" };
        var res = await client.PostAsync("/api/auth/refresh",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}