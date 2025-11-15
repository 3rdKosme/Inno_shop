namespace Inno_Shop.UserService.Application.Abstractions;

public interface ICurrentUserService
{
    int? UserId { get; }

    string? Role {  get; }
}