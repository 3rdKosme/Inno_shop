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
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole UserRole { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = new();
        public List<PasswordResetToken> PasswordResetTokens { get; set; } = new();
        public List<EmailConfirmationToken> EmailConfirmationTokens { get; set; } = new();

        public User() { }

        public static User Create(string name, string email, string passwordHash)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Guard.AgainstNullOrWhiteSpace(email, nameof(email));
            Guard.AgainstNullOrWhiteSpace(passwordHash, nameof(passwordHash));

            return new User()
            {
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                UserRole = UserRole.User,
                IsEmailConfirmed = false,
                IsActive = true,
                IsLocked = false,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public void ChangeName(string name)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Name = name;
        }

        public void ChangeEmail(string email)
        {
            Guard.AgainstNullOrWhiteSpace(email, nameof(email));
            Email = Email;
            IsEmailConfirmed = false;
        }

        public void ChangePassword(string passwordHash)
        {
            Guard.AgainstNullOrWhiteSpace(passwordHash, nameof(passwordHash));
            PasswordHash = passwordHash;
        }

        public void PromoteToAdmin()
        {
            UserRole = UserRole.Admin;
        }
        public void ConfirmEmail()
        {
            if (IsEmailConfirmed) throw new EmailAlreadyConfirmedException();
            IsEmailConfirmed = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new AlreadyDoneException();
            }
            IsActive = false;
        }
        public void Activate() {
            if (IsActive)
            {
                throw new AlreadyDoneException();
            }
            IsActive = true;
        }

        public void Lock()
        {
            if (IsLocked) {
                throw new AlreadyDoneException();
            }
            IsLocked = true; 
        }

        public void Unlock()
        {
            if (!IsLocked)
            {
                throw new AlreadyDoneException();
            }
            IsLocked = false;
        }
    }
}
