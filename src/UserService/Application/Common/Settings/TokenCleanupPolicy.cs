using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.UserService.Application.Common.Settings;

public class TokenCleanupPolicy
{
    public int ExecutionIntervalMinutes { get; set; }
    public int ExpirationGracePeriodHours { get; set; }
}