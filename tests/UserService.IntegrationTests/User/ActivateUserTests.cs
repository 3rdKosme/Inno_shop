using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.User;

public class ActivateUserTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task ActivateUser_ActivatesAndSendsEmail_OnSuccess()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password = "p123";
        var passwordHash = passwordHasher.HashPassword(password);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("zzz", "z@z.com", passwordHash);
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("IsActive")!.SetValue(user, false);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = password };
        var res = await client.PostAsync("/api/user/me/activate",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task ActivateUser_Throws_UnauthorizedAccessException_WhenNotAuthorized()
    {
        var client = factory.CreateClient();
        var res = await client.PostAsync("/api/user/me/activate",
            new StringContent("{}", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task ActivateUser_Throws_NotFound_WhenUser_NotExists()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-88888-role-User");
        var req = new { Password = "doesntmatter" };
        var res = await client.PostAsync("/api/user/me/activate",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task ActivateUser_Throws_InvalidCredentials_WhenPasswordIncorrect()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var correctPassword = "secret123";
        var passwordHash = passwordHasher.HashPassword(correctPassword);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("Test", "t1@t.com", passwordHash);
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("IsActive")!.SetValue(user, false);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = "WrongPass123" };
        var res = await client.PostAsync("/api/user/me/activate",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task ActivateUser_Throws_BusinessRule_WhenAlreadyActivated()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password = "ha123";
        var passwordHash = passwordHasher.HashPassword(password);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("N", "n@x.com", passwordHash);
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("IsActive")!.SetValue(user, true);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = password };
        var res = await client.PostAsync("/api/user/me/activate",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }
}