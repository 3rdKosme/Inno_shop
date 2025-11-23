using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.DTOs;
using Inno_Shop.ProductService.Domain.Entities;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(IProductReadRepository productRepository) : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductReadRepository _productRepository = productRepository;

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken = default)
    {
        
    }
}