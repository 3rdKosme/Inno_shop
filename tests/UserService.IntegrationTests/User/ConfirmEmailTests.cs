using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.User;

public class ConfirmEmailTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task ConfirmEmail_Confirms_On_ValidToken()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("T1", "te@te.com", "hhh");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var tokenStr = "valid-token";
        var token = new EmailConfirmationToken(user.Id, tokenStr, DateTime.UtcNow.AddMinutes(15));
        db.EmailConfirmationTokens.Add(token);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Token = tokenStr };
        var res = await client.PostAsync("/api/user/me/confirmEmail",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.True(res.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ConfirmEmail_Throws_InvalidCredentials_WhenTokenWrong()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("T2", "t2@t2.com", "hash");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Token = "not-exist-token" };
        var res = await client.PostAsync("/api/user/me/confirmEmail",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmail_Throws_TokenIsExpiredOrRevoked_WhenExpiredOrRevoked()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("V2", "v@v.com", "a");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var tokenStr = "expired-token";
        var expiredToken = new EmailConfirmationToken(user.Id, tokenStr, DateTime.UtcNow.AddMinutes(-5));
        db.EmailConfirmationTokens.Add(expiredToken);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Token = tokenStr };
        var res = await client.PostAsync("/api/user/me/confirmEmail",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ConfirmEmail_Throws_NotFound_WhenUserNotFound()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenStr = "nouser-token";
        var token = new EmailConfirmationToken(7777, tokenStr, DateTime.UtcNow.AddMinutes(10));
        db.EmailConfirmationTokens.Add(token);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-7777-role-User");
        var req = new { Token = tokenStr };
        var res = await client.PostAsync("/api/user/me/confirmEmail",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmail_Throws_BusinessRule_WhenEmailAlreadyConfirmed()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("A", "a@a.com", "x");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("IsEmailConfirmed")!.SetValue(user, true);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var tokenStr = "confirmed-token";
        var token = new EmailConfirmationToken(user.Id, tokenStr, DateTime.UtcNow.AddMinutes(20));
        db.EmailConfirmationTokens.Add(token);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Token = tokenStr };
        var res = await client.PostAsync("/api/user/me/confirmEmail",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }
}