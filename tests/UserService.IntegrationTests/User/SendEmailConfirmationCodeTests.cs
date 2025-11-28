using System.Net;
using System.Net.Http.Headers;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.IntegrationTests.User;

public class SendEmailConfirmationCodeTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task SendEmailConfirmationCode_Sends_On_Success()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = Inno_Shop.UserService.Domain.Entities.User.Create("T", "test@example.com", "x");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            $"test-jwt-token-for-{user.Id}-role-User-email-test@example.com");
        var res = await client.PostAsync("/api/user/me/sendEmailConfirmationCode", null);
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task SendEmailConfirmationCode_Throws_UnauthorizedAccessException_WhenNotAuthorized()
    {
        var client = factory.CreateClient();
        var res = await client.PostAsync("/api/user/me/sendEmailConfirmationCode", null);
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task SendEmailConfirmationCode_Throws_NotFound_WhenUser_NotExists()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer",
                "test-jwt-token-for-55555-email-test@example.com");
        var res = await client.PostAsync("/api/user/me/sendEmailConfirmationCode", null);
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}