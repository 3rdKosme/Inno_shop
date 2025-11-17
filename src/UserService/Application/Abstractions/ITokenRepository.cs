using Inno_Shop.UserService.Domain.Entities;

namespace Inno_Shop.UserService.Application.Abstractions;

public interface ITokenRepository<T> : IRepository<T> where T : BaseToken
{
    public Task<T?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    public Task<IEnumerable<BaseToken>> GetObsoleteTokensAsync(DateTime threshold, CancellationToken cancellationToken = default);
}