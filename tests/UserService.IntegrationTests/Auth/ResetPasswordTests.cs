using System.Text;
using System.Text.Json;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.IntegrationTests.Auth;

public class ResetPasswordTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task ResetPassword_Resets_OnValidToken()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("HasReset", "reset@do.com", "old");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var tokenStr = "valid-reset-token";
        var token = new PasswordResetToken(user.Id, tokenStr, DateTime.UtcNow.AddMinutes(10));
        db.PasswordResetTokens.Add(token);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        var req = new { Token = tokenStr, NewPassword = "NEWpassword111" };
        var res = await client.PostAsync("/api/auth/resetPassword",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.True(res.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ResetPassword_ReturnsError_OnInvalidToken()
    {
        var client = factory.CreateClient();
        var req = new { Token = "wrong", NewPassword = "ValidPass123" };
        var res = await client.PostAsync("/api/auth/resetPassword",
            new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        Assert.False(res.IsSuccessStatusCode);
    }
}