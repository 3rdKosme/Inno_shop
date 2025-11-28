using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Domain.Enums;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.IntegrationTests.Admin;

public class UpdateUserTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task UpdateUser_Success_WhenValidAndAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("AdminU", "ad@a.com", "pw");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        var u = Inno_Shop.UserService.Domain.Entities.User.Create("ToUpd", "to@up.com", "w");
        db.Users.Add(admin);
        db.Users.Add(u);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var req = new { Name = "NEWADMINNAME" };
        var res = await client.PutAsync($"/api/admin/user/{u.Id}",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_NotFound_WhenUserNotExist()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("X1", "x@ad.com", "z");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var req = new { Name = "Nope" };
        var res = await client.PutAsync("/api/admin/user/45000",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_Forbidden_WhenNotAdmin()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var u1 = Inno_Shop.UserService.Domain.Entities.User.Create("NN", "nn@u.com", "p");
        db.Users.Add(u1);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{u1.Id}-role-User");
        var req = new { Name = "Blocked" };
        var res = await client.PutAsync($"/api/admin/user/{u1.Id}",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_BusinessRule_WhenInvalidName()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = Inno_Shop.UserService.Domain.Entities.User.Create("BNN", "b@ad.com", "sdfg");
        typeof(Inno_Shop.UserService.Domain.Entities.User).GetProperty("UserRole")!.SetValue(admin, UserRole.Admin);
        var u = Inno_Shop.UserService.Domain.Entities.User.Create("ToUpd2", "to2@up.com", "1");
        db.Users.Add(admin);
        db.Users.Add(u);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{admin.Id}-role-Admin");
        var req = new { Name = "" };
        var res = await client.PutAsync($"/api/admin/user/{u.Id}",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }
}