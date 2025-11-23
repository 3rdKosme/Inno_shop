using Inno_Shop.ProductService.Domain.Entities;

namespace Inno_Shop.ProductService.Application.Abstractions;

public interface IProductWriteRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
}