using Inno_Shop.UserService.Domain.Entities;

namespace Inno_Shop.UserService.Application.Abstractions;

public interface IRepository<T> where T : BaseEntity
{
    public Task AddAsync(T entity, CancellationToken cancellationToken = default);
    public Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);
}