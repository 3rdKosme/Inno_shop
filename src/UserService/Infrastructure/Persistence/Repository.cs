using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;

namespace Inno_Shop.UserService.Infrastructure.Persistence;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
{
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        context.Set<T>().Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var existing = await context.Set<T>().FindAsync([entity.Id], cancellationToken);
        if(existing ==  null) return false;

        context.Entry(existing).CurrentValues.SetValues(entity);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}