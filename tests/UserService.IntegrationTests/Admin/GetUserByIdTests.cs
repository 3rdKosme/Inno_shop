using System.Net;
using System.Net.Http.Headers;
using Inno_Shop.UserService.Domain.Enums;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.Admin;

public class GetUserByIdTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetUserById_ReturnsUser_WhenExistsAndAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("A1", "admin@ad.com", "y");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("Normal", "any@b.com", "pa");
        db.Users.Add(admin);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.GetAsync($"/api/admin/user/{user.Id}");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task GetUserById_NotFound_WhenUserNotExist()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("A1", "admin2@ad.com", "y");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.GetAsync("/api/admin/user/55555");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task GetUserById_Forbidden_WhenNotAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("U", "u@us.com", "u");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");
        var res = await client.GetAsync($"/api/admin/user/{user.Id}");
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }
}