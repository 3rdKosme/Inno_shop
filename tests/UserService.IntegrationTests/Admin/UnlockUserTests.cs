using System.Net;
using System.Net.Http.Headers;
using Inno_Shop.UserService.Domain.Enums;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.Admin;

public class UnlockUserTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task UnlockUser_Success_WhenValidAndAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Domain.Entities.User.Create("Adm1", "adm1@adm.com", "1");
        typeof(Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        var u = Domain.Entities.User.Create("lockX", "b@b.com", "c");
        typeof(Domain.Entities.User).GetProperty("IsLocked")!.SetValue(u, true);
        db.Users.Add(admin);
        db.Users.Add(u);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.PostAsync($"/api/admin/user/{u.Id}/unlock", null);
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task UnlockUser_NotFound_WhenUserNotExist()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Domain.Entities.User.Create("Adm2", "adm2@adm.com", "2");
        typeof(Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.PostAsync("/api/admin/user/9999/unlock", null);
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task UnlockUser_Forbidden_WhenNotAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var u1 = Domain.Entities.User.Create("U3", "u3@u.com", "r");
        var u2 = Domain.Entities.User.Create("U2", "u2@u.com", "x");
        typeof(Domain.Entities.User).GetProperty("IsLocked")!.SetValue(u2, true);
        db.Users.Add(u1);
        db.Users.Add(u2);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{u1.Id}-role-User");
        var res = await client.PostAsync($"/api/admin/user/{u2.Id}/unlock", null);
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task UnlockUser_BusinessRule_WhenNotLocked()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Domain.Entities.User.Create("Adm4", "adm4@adm.com", "4");
        typeof(Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        var u = Domain.Entities.User.Create("NotLocked", "notLocked@b.com", "pw4");
        typeof(Domain.Entities.User).GetProperty("IsLocked")!.SetValue(u, false);
        db.Users.Add(admin);
        db.Users.Add(u);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var res = await client.PostAsync($"/api/admin/user/{u.Id}/unlock", null);
        Assert.False(res.IsSuccessStatusCode);
    }
}