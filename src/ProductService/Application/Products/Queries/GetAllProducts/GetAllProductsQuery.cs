using MediatR;
using Inno_Shop.ProductService.Application.DTOs;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetAllProducts;

public sealed record GetAllProductsQuery(
    string? Search, 
    double? MaxPrice, 
    double? MinPrice, 
    bool? IsAvailable,
    int? UserId, 
    string? SortBy, 
    string? Direction, 
    int Page, 
    int PageSize
    ) : IRequest<IEnumerable<ProductDto>>;