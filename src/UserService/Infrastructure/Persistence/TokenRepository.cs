using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Inno_Shop.UserService.Application.Common.Constants;
using System.Reflection.Metadata.Ecma335;

namespace Inno_Shop.UserService.Infrastructure.Persistence;

public class TokenRepository<T>(AppDbContext context) : Repository<T>(context), ITokenRepository<T> where T : BaseEntity
{
    public readonly AppDbContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public virtual async Task<T?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(token, cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}