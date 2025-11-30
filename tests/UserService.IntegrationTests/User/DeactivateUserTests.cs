using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.User;

public class DeactivateUserTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task DeactivateUser_DeactivatesAndSendsEmail_OnSuccess()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password = "pass123";
        var passwordHash = passwordHasher.HashPassword(password);
        var user = Domain.Entities.User.Create("Active", "active@user.com", passwordHash);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = password };
        var res = await client.PostAsync("/api/user/me/deactivate",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task DeactivateUser_Throws_UnauthorizedAccessException_WhenNotAuthorized()
    {
        var client = factory.CreateClient();
        var res = await client.PostAsync("/api/user/me/deactivate",
            new StringContent("{}", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task DeactivateUser_Throws_NotFound_WhenUser_NotExists()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-9999999-role-User");
        var req = new { Password = "aPw123456" };
        var res = await client.PostAsync("/api/user/me/deactivate",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task DeactivateUser_Throws_InvalidCredentials_WhenPasswordIncorrect()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var correctPassword = "pwdpass123";
        var passwordHash = passwordHasher.HashPassword(correctPassword);
        var user = Domain.Entities.User.Create("ToPwd", "d@u.com", passwordHash);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = "notapass" };
        var res = await client.PostAsync("/api/user/me/deactivate",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task DeactivateUser_Throws_BusinessRule_WhenAlreadyDeactivated()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password = "zz123";
        var passwordHash = passwordHasher.HashPassword(password);
        var user = Domain.Entities.User.Create("Xxx", "a@b.com", passwordHash);
        typeof(Domain.Entities.User).GetProperty("IsActive")!.SetValue(user, false);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = password };
        var res = await client.PostAsync("/api/user/me/deactivate",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }
}