namespace Inno_Shop.UserService.Domain.Entities
{
    public class BaseToken : BaseEntity
    {
        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }

        protected BaseToken() { }

        public BaseToken(int userId, string token, DateTime expiresAt)
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
}
