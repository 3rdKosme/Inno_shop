using Inno_Shop.UserService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void ConfirmEmail()
        {
            if (IsEmailConfirmed) throw new Exception;
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
