namespace Inno_Shop.UserService.Application.Common.Settings;

public class TokenCleanupPolicy
{
    public int ExecutionIntervalMinutes { get; set; }
    public int ExpirationGracePeriodHours { get; set; }
}