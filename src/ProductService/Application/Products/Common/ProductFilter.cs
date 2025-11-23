namespace Inno_Shop.ProductService.Application.Products.Common;

public record ProductFilter(
    string? Search,
    double? MaxPrice,
    double? MinPrice,
    bool? IsAvailable,
    int? UserId,
    string? Sort,
    int Page,
    int PageSize);