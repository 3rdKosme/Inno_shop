using System.Net;
using System.Net.Http.Headers;
using Inno_Shop.UserService.Domain.Enums;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.IntegrationTests.Admin;

public class LockUserTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task LockUser_Success_WhenValidAndAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("AA", "aa@ad.com", "ww");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        var u = Inno_Shop.UserService.Domain.Entities.User.Create("LockMe", "lock@t.com", "11");
        db.Users.Add(admin);
        db.Users.Add(u);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.PostAsync($"/api/admin/user/{u.Id}/lock", null);
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task LockUser_NotFound_WhenUserNotExist()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("NNN", "dn@adm.com", "zz");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.PostAsync("/api/admin/user/96666/lock", null);
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task LockUser_Forbidden_WhenNotAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user1 = Inno_Shop.UserService.Domain.Entities.User.Create("Abc", "abc@a.com", "w");
        var user2 = Inno_Shop.UserService.Domain.Entities.User.Create("b", "b@b.com", "er");
        db.Users.Add(user1);
        db.Users.Add(user2);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user1.Id}-role-User");
        var res = await client.PostAsync($"/api/admin/user/{user2.Id}/lock", null);
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task LockUser_BusinessRule_WhenAlreadyLocked()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("AA2", "aa2@ad.com", "zz");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        var u = Inno_Shop.UserService.Domain.Entities.User.Create("Locked", "lock2@t.com", "77");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("IsLocked")!.SetValue(u, true);
        db.Users.Add(admin);
        db.Users.Add(u);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.PostAsync($"/api/admin/user/{u.Id}/lock", null);
        Assert.False(res.IsSuccessStatusCode);
    }
}