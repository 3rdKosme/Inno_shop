namespace Inno_Shop.UserService.Application.Abstractions;

public interface ITokenRepository<T> : IRepository<T> where T : class
{
    public Task<T?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}