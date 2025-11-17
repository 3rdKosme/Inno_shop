namespace Inno_Shop.UserService.Application.Abstractions;

public interface ITokenCleanupService
{
    Task CleanupAsync();
}