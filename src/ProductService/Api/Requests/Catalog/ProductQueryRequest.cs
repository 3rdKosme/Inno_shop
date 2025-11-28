using Inno_Shop.ProductService.Application.Products.Queries.GetProducts;

namespace Inno_Shop.ProductService.Api.Requests.Catalog;

public record ProductQueryRequest(
    string? Search,
    double? MaxPrice,
    double? MinPrice,
    bool? IsAvailable,
    int? UserId,
    string? Sort,
    int Page = 1,
    int PageSize = 20)
{
    public GetProductsQuery ToQuery() => new (Search: Search, MaxPrice: MaxPrice, 
        MinPrice: MinPrice, IsAvailable: IsAvailable, UserId: UserId, Sort: Sort, 
        Page: Page, PageSize: PageSize);
}