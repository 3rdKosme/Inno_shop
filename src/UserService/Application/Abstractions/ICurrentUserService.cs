namespace Inno_Shop.UserService.Application.Abstractions;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Email { get; }
    string? Role {  get; }
}