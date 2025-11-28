using System.Net;
using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.IntegrationTests.Auth;

public class SendPasswordResetCodeTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task ForgotPassword_SendsEmail_OnValidEmail()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("Any", "forgot@auth.com", "hhh");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        var req = new { Email = "forgot@auth.com" };
        var res = await client.PostAsync("/api/auth/forgotPassword",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.True(res.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ForgotPassword_Returns404_OnUserNotFound()
    {
        var client = factory.CreateClient();
        var req = new { Email = "none@f.com" };
        var res = await client.PostAsync("/api/auth/forgotPassword",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}