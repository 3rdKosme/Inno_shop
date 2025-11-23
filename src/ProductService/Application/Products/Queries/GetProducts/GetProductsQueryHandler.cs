using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.DTOs;
using Inno_Shop.ProductService.Domain.Entities;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken = default)
    {
        var filter = request.ToFilter();
        var products = await productRepository.GetProductsAsync(request.ToFilter(), cancellationToken);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.IsAvailable,
            p.UserId,
            p.CreatedAt
        ));
    }
}