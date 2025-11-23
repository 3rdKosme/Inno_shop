namespace Inno_Shop.UserService.Application.Abstractions;

public interface ITokenCleanupService
{
    public Task CleanupAsync();
}