using Inno_Shop.ProductService.Application.DTOs;

namespace Inno_Shop.ProductService.Application.Abstractions;

public interface IProductReadRepository
{
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
}