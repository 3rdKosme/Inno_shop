using MediatR;
using Inno_Shop.ProductService.Application.DTOs;
using Inno_Shop.ProductService.Application.Products.Common;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    string? Search,
    double? MaxPrice,
    double? MinPrice,
    bool? IsAvailable,
    int? UserId,
    string? Sort,
    int Page,
    int PageSize
) : IRequest<IEnumerable<ProductDto>>
{
    public ProductFilter ToFilter() => new (Search: Search, MaxPrice: MaxPrice, 
        MinPrice: MinPrice, IsAvailable: IsAvailable, UserId: UserId, Sort: Sort, 
        Page: Page, PageSize: PageSize);
}