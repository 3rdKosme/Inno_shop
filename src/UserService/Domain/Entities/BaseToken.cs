namespace Inno_Shop.UserService.Domain.Entities;

public abstract class BaseToken : BaseEntity
{
    public int UserId { get; init; }
    public string Token { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsRevoked { get; private set; }

    protected BaseToken() { }

    protected BaseToken(int userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public void Revoke()
    {
        IsRevoked = true;
    }
}

