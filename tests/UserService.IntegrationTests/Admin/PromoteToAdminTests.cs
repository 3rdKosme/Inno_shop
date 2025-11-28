using System.Net;
using System.Net.Http.Headers;
using Inno_Shop.UserService.Domain.Enums;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.IntegrationTests.Admin;

public class PromoteToAdminTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task PromoteToAdmin_Success_WhenValidAndAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("PA1", "pa@a.com", "1");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        var u = Inno_Shop.UserService.Domain.Entities.User.Create("MakeAdmin", "m@m.com", "p");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(u, UserRole.User);
        db.Users.Add(admin);
        db.Users.Add(u);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.PostAsync($"/api/admin/user/{u.Id}/promoteToAdmin", null);
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task PromoteToAdmin_NotFound_WhenUserNotExist()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("PA2", "pa2@a.com", "2");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.PostAsync("/api/admin/user/43242/promoteToAdmin", null);
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task PromoteToAdmin_Forbidden_WhenNotAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user1 = Inno_Shop.UserService.Domain.Entities.User.Create("Y1", "y1@y.com", "r");
        var user2 = Inno_Shop.UserService.Domain.Entities.User.Create("YY", "yy@y.com", "s");
        db.Users.Add(user1);
        db.Users.Add(user2);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user1.Id}-role-User");
        var res = await client.PostAsync($"/api/admin/user/{user2.Id}/promoteToAdmin", null);
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task PromoteToAdmin_BusinessRule_WhenAlreadyAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("A1", "a1@a.com", "p");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        var u = Inno_Shop.UserService.Domain.Entities.User.Create("ADM", "adm@b.com", "w");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(u, UserRole.Admin);
        db.Users.Add(admin);
        db.Users.Add(u);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.PostAsync($"/api/admin/user/{u.Id}/promoteToAdmin", null);
        Assert.False(res.IsSuccessStatusCode);
    }
}