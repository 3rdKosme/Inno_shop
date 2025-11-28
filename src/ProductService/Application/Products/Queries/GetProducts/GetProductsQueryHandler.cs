using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.DTOs;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler(IProductRepository productRepository) 
    : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken = default)
    {
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