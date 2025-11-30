using System.Net;
using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.Auth;

public class LoginTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Login_ReturnsToken_OnValidCredentials()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password = "password123";
        var passwordHash = passwordHasher.HashPassword(password);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("U", "u@a.com", passwordHash);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        var req = new { Email = "u@a.com", Password = password };
        var res = await client.PostAsync("/api/auth/login",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Login_Returns401_OnWrongPassword()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var correctPassword = "correctpass";
        var passwordHash = passwordHasher.HashPassword(correctPassword);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("U", "qqq@a.com", passwordHash);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        var req = new { Email = "qqq@a.com", Password = "wrongpass" };
        var res = await client.PostAsync("/api/auth/login",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}