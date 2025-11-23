using Inno_Shop.UserService.Domain.Enums;
using Inno_Shop.UserService.Domain.Common;
using Inno_Shop.UserService.Domain.Common.Constants;
using Inno_Shop.UserService.Domain.Common.Exceptions;

namespace Inno_Shop.UserService.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole UserRole { get; private set; }
        public bool IsEmailConfirmed { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsLocked { get; private set; }
        public DateTime CreatedAt { get; init; }
        public List<RefreshToken> RefreshTokens { get; init; } = [];
        public List<PasswordResetToken> PasswordResetTokens { get; init; } = [];
        public List<EmailConfirmationToken> EmailConfirmationTokens { get; init; } = [];

        private User() { }

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
            Email = email;
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
            if (IsEmailConfirmed) throw new EmailAlreadyConfirmedException(ErrorMessages.EmailAlreadyConfirmed);
            IsEmailConfirmed = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new AlreadyDoneException(ErrorMessages.AlreadyDeactivated);
            }
            IsActive = false;
        }
        public void Activate() {
            if (IsActive)
            {
                throw new AlreadyDoneException(ErrorMessages.AlreadyActivated);
            }
            IsActive = true;
        }

        public void Lock()
        {
            if (IsLocked) {
                throw new AlreadyDoneException(ErrorMessages.AlreadyLocked);
            }
            IsLocked = true; 
        }

        public void Unlock()
        {
            if (!IsLocked)
            {
                throw new AlreadyDoneException(ErrorMessages.AlreadyUnlocked);
            }
            IsLocked = false;
        }
    }
}
