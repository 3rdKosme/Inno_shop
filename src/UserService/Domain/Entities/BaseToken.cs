using Inno_Shop.UserService.Domain.Enums;
using Inno_Shop.UserService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inno_Shop.UserService.Domain.Common.Exceptions;

namespace Inno_Shop.UserService.Domain.Entities
{
    public class BaseToken : BaseEntity
    {
        public int UserId { get; set; }
        public string Token { get; set; }
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
