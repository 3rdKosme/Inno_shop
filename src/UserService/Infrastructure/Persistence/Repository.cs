using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Inno_Shop.UserService.Infrastructure.Persistence;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
{
    public readonly AppDbContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var existing = await _dbSet.FindAsync(new object[] { entity.Id }, cancellationToken);
        if(existing ==  null) return false;

        _context.Entry(existing).CurrentValues.SetValues(entity);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}