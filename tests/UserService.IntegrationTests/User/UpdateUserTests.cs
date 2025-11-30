using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.User;

public class UpdateUserTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task UpdateUser_Updates_User_On_Valid_Data()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password = "validpass123";
        var passwordHash = passwordHasher.HashPassword(password);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("Main", "main@user.com", passwordHash);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        var requestData = new
            { Password = password, Name = "NewName", Email = "new@user.com", NewPassword = "NewPa$$123" };
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var res = await client.PutAsync("/api/user/me",
            new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_Throws_UnauthorizedAccessException_WhenNotAuthorized()
    {
        var client = factory.CreateClient();
        var res = await client.PutAsync("/api/user/me", new StringContent("{}", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_Throws_NotFound_WhenUser_NotExists()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "test-jwt-token-for-9999-role-User");
        var req = new { Password = "Anything123", Name = "Any", Email = "em@any.com" };
        var res = await client.PutAsync("/api/user/me",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_Throws_InvalidCredentials_WhenPasswordIncorrect()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var correctPassword = "realpass123";
        var passwordHash = passwordHasher.HashPassword(correctPassword);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("Name", "em@fail.com", passwordHash);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = "Incorrect123", Name = "ok", Email = "test@email.com" };
        var res = await client.PutAsync("/api/user/me",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_Throws_BusinessRule_WhenInvalidName()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password = "pw1234567";
        var passwordHash = passwordHasher.HashPassword(password);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("NotEmpty", "bus@email.com", passwordHash);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = password, Name = "", Email = "bus@test.com" };
        var res = await client.PutAsync("/api/user/me",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateUser_Throws_EmailAlreadyExists_WhenEmailExists()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password1 = "p1pass123";
        var password2 = "p2pass123";
        var passwordHash1 = passwordHasher.HashPassword(password1);
        var passwordHash2 = passwordHasher.HashPassword(password2);
        var user1 = Inno_Shop.UserService.Domain.Entities.User.Create("1", "exist@email.com", passwordHash1);
        var user2 = Inno_Shop.UserService.Domain.Entities.User.Create("2", "other@email.com", passwordHash2);
        db.Users.Add(user1);
        db.Users.Add(user2);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user2.Id}-role-User");
        var req = new { Password = password2, Name = "any", Email = "exist@email.com" };
        var res = await client.PutAsync("/api/user/me",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateUser_Throws_BusinessRule_WhenInvalidEmail()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password = "pass123";
        var passwordHash = passwordHasher.HashPassword(password);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("Email", "emf@err.com", passwordHash);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = password, Name = "Name", Email = "", NewPassword = "Abc12345" };
        var res = await client.PutAsync("/api/user/me",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateUser_Throws_BusinessRule_WhenInvalidPassword()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var password = "pwd123";
        var passwordHash = passwordHasher.HashPassword(password);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("Some", "some@email.com", passwordHash);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var req = new { Password = password, Name = "Some", Email = "some@email.com", NewPassword = "" };
        var res = await client.PutAsync("/api/user/me",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }
}