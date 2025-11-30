using System.Net;
using System.Net.Http.Headers;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inno_Shop.UserService.IntegrationTests.User;

public class GetCurrentUserTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetCurrentUser_Returns_UserDto_WhenAuthorized()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = Domain.Entities.User.Create("Test User", "testuser@mail.com", "hash");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"test-jwt-token-for-{user.Id}-role-User");

        var response = await client.GetAsync("/api/user/me");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_Throws_UnauthorizedAccessException_WhenNotAuthorized()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/user/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}