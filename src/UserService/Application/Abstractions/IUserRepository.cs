using Inno_Shop.UserService.Domain.Entities;

namespace Inno_Shop.UserService.Application.Abstractions;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    public Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}