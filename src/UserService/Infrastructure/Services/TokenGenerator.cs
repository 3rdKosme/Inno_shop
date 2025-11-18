using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Inno_Shop.UserService.Infrastructure.Services;

public class TokenGenerator(IOptions<TokenGeneratorSettings> tokenGeneratorSettings) : ITokenGenerator
{
    private readonly TokenGeneratorSettings _tokenGeneratorSettings = tokenGeneratorSettings.Value;

    public string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(_tokenGeneratorSettings.TokenLength);
        return Convert.ToBase64String(bytes)
                     .Replace('+', '-')
                     .Replace('/', '_')
                     .TrimEnd('=');
    }
}