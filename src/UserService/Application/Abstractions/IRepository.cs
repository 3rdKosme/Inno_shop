namespace Inno_Shop.UserService.Application.Abstractions;

public interface IRepository<T> where T : class
{
    public Task AddAsync(T entity, CancellationToken cancellationToken = default);
    public Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);
}