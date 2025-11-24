namespace Inno_Shop.Shared.Application.Abstractions;

public interface ICurrentUserService
{
    public int? UserId { get; }
    public string? Email { get; }
}