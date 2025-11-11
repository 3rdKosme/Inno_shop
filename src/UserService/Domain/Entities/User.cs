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
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole UserRole { get; private set; }
        public bool IsEmailConfirmed { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

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
                CreatedAt = DateTime.Now,
            };
        }
        public void ConfirmEmail()
        {
            if (IsEmailConfirmed) throw new EmailAlreadyConfirmedException();
            IsEmailConfirmed = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
        public void Activate() {
            IsActive = true;
        }
    }
}
