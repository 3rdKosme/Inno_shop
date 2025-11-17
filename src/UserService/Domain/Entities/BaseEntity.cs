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
    public class BaseEntity
    {
        public int Id { get; set; }
    }
}
