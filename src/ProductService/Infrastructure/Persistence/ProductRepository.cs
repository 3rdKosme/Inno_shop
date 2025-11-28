using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Inno_Shop.ProductService.Application.Products.Common;

namespace Inno_Shop.ProductService.Infrastructure.Persistence;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<IEnumerable<Product>> GetProductsAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        var q = context.Products.Where(p => !p.IsDeleted).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            q = q.Where(p => p.Name.Contains(filter.Search) || p.Description.Contains(filter.Search));
        }

        if (filter.MinPrice is not null)
        {
            q = q.Where(p=> p.Price >=  filter.MinPrice);
        }

        if (filter.MaxPrice is not null)
        {
            q = q.Where(p => p.Price <= filter.MaxPrice);
        }

        if (filter.IsAvailable is not null)
        {
            q = q.Where(p => p.IsAvailable == filter.IsAvailable);
        }

        if (filter.UserId is not null)
        {
            q = q.Where(p => p.UserId == filter.UserId);
        }

        q = filter.Sort switch
        {
            "price_asc" => q.OrderBy(p => p.Price),
            "price_desc" => q.OrderByDescending(p => p.Price),
            "created_asc" => q.OrderBy(p => p.CreatedAt),
            "created_desc" => q.OrderByDescending(p => p.CreatedAt),
            _ => q.OrderBy(p => p.Id)
        };
        
        q = q.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);
        
        return await q.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await context.Products.Where(p => p.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Products.FindAsync([id], cancellationToken);
    }

    public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        await context.Products.AddAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
    }

    public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
    }
}