using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inno_Shop.UserService.Infrastructure.Options;
using Inno_Shop.UserService.Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;

namespace Inno_Shop.UserService.Infrastructure.UnitTests.Services;

public class JwtTokenServiceTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtTokenService _service;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtTokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            Key = "THIS_IS_SUPER_SECRET_JWT_KEY_1234567890",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpireMinutes = 30
        };
        _service = new JwtTokenService(Microsoft.Extensions.Options.Options.Create(_jwtSettings));
    }

    [Fact]
    public void GenerateAccessToken_Should_Return_Valid_JWT()
    {
        var userId = 42;
        var email = "john@example.com";
        var role = "Admin";
        
        var token = _service.GenerateAccessToken(userId, email, role);
        
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var jwt = _tokenHandler.ReadJwtToken(token);

        Assert.Equal(_jwtSettings.Issuer, jwt.Issuer);
        Assert.Equal(_jwtSettings.Audience, jwt.Audiences.Single());

        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == role);
        
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateAccessToken_Should_Have_Correct_SigningAlgorithm()
    {
        var token = _service.GenerateAccessToken(1, "a@a.com", "User");

        var jwt = _tokenHandler.ReadJwtToken(token);

        Assert.Equal(SecurityAlgorithms.HmacSha256, jwt.Header.Alg);
    }

    [Fact]
    public void GenerateAccessToken_Should_Produce_Token_With_ValidSignature()
    {
        var token = _service.GenerateAccessToken(1, "test@test.com", "User");

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
        };
        
        _tokenHandler.ValidateToken(token, validationParameters, out _);
    }

    [Fact]
    public void GenerateAccessToken_Should_Throw_When_Email_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _service.GenerateAccessToken(1, null!, "User"));
    }

    [Fact]
    public void GenerateRefreshToken_Should_Return_Base64_String()
    {
        var token = _service.GenerateRefreshToken();

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var bytes = Convert.FromBase64String(token);
        Assert.Equal(32, bytes.Length);
    }

    [Fact]
    public void GenerateRefreshToken_Should_Return_Unique_Values()
    {
        var token1 = _service.GenerateRefreshToken();
        var token2 = _service.GenerateRefreshToken();

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateRefreshToken_Should_Not_Be_Deterministic()
    {
        var results = new HashSet<string>();

        for (var i = 0; i < 5; i++)
            results.Add(_service.GenerateRefreshToken());

        Assert.Equal(5, results.Count);
    }
}