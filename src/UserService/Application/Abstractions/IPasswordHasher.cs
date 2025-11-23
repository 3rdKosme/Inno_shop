namespace Inno_Shop.UserService.Application.Abstractions;

public interface IPasswordHasher
{
    public string HashPassword(string password);
    public bool VerifyPassword(string password, string hash);
}