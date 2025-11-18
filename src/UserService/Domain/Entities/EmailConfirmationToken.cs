namespace Inno_Shop.UserService.Domain.Entities;

public class EmailConfirmationToken : BaseToken  
{
    protected EmailConfirmationToken() { }

    public EmailConfirmationToken(int userId, string token, DateTime expiresAt) : base(userId, token, expiresAt)
    {

    }
}