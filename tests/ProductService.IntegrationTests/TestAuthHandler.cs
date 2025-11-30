using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Inno_Shop.ProductService.IntegrationTests;

public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Bearer "))
            return AuthenticateResult.Fail("No Test Token");

        var token = header.Substring("Bearer ".Length);

        var regex = new Regex(
            @"test-jwt-token-for-(\d+)(-role-(Admin|User))?",
            RegexOptions.IgnoreCase);

        var match = regex.Match(token);

        if (!match.Success)
            return AuthenticateResult.Fail("Wrong test token format");

        var userIdStr = match.Groups[1].Value;
        var role = match.Groups[3].Success ? match.Groups[3].Value : "User";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userIdStr),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}