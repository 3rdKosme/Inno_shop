using Inno_Shop.UserService.Domain.Entities;

namespace Inno_Shop.UserService.Application.Abstractions;

public interface IUserRepository
{
    //public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    public Task AddAsync(User user, CancellationToken cancellationToken = default);
    public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}