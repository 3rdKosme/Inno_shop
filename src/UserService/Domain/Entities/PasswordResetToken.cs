namespace Inno_Shop.UserService.Domain.Entities;

public class PasswordResetToken : BaseToken
{
    protected PasswordResetToken() { }

    public PasswordResetToken(int userId, string token, DateTime expiresAt) : base(userId, token, expiresAt)
    {

    }
}