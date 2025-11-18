namespace Inno_Shop.UserService.Domain.Entities;

public class RefreshToken : BaseToken
{
    protected RefreshToken() { }

    public RefreshToken(int userId, string token, DateTime expiresAt) : base(userId, token, expiresAt)
    {

    }
}