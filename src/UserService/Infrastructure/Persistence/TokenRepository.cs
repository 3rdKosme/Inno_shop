using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Inno_Shop.UserService.Infrastructure.Persistence;

public class TokenRepository<T>(AppDbContext context) : Repository<T>(context), ITokenRepository<T> where T : BaseToken
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public virtual async Task<T?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public virtual async Task<IEnumerable<BaseToken>> GetObsoleteTokensAsync(DateTime threshold, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsQueryable().Where(t => t.IsRevoked || t.ExpiresAt < DateTime.UtcNow && t.ExpiresAt < threshold).ToListAsync(cancellationToken);
    }
}