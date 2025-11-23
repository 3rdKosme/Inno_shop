using Inno_Shop.ProductService.Application.Products.Common;
using Inno_Shop.ProductService.Domain.Entities;

namespace Inno_Shop.ProductService.Application.Abstractions;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(ProductFilter filter, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddProductAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
}