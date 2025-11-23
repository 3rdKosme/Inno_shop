namespace Inno_Shop.UserService.Application.Common.Settings;

public class TokenCleanupPolicy
{
    public required int ExecutionIntervalMinutes { get; init; }
    public required int ExpirationGracePeriodHours { get; init; }
}