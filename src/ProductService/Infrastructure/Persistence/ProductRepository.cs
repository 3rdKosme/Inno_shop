using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inno_Shop.ProductService.Infrastructure.Persistence;

public class ProductWriteRepository(AppDbContext context) : IProductWriteRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FindAsync(id, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}