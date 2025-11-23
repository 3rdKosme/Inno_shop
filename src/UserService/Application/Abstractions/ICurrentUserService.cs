namespace Inno_Shop.UserService.Application.Abstractions;

public interface ICurrentUserService
{
    public int? UserId { get; }
    public string? Email { get; }
}