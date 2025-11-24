using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Inno_Shop.UserService.Infrastructure.Persistence;

public class TokenRepository<T>(AppDbContext context) : Repository<T>(context), ITokenRepository<T> where T : BaseToken
{

    public virtual async Task<T?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await context.Set<T>().FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Set<T>().FindAsync([id], cancellationToken);
        if (entity == null) return false;

        context.Set<T>().Remove(entity);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public virtual async Task<IEnumerable<T>> GetObsoleteTokensAsync(DateTime threshold, CancellationToken cancellationToken = default)
    {
        return await context.Set<T>().AsQueryable().Where(t => t.IsRevoked || t.ExpiresAt < DateTime.UtcNow && t.ExpiresAt < threshold).ToListAsync(cancellationToken);
    }
}