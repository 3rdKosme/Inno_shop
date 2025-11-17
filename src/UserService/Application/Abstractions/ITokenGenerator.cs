namespace Inno_Shop.UserService.Application.Abstractions;

public interface ITokenGenerator
{
    public string GenerateSecureToken();
}