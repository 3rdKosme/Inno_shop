using System.Security.Cryptography.X509Certificates;

namespace Inno_Shop.ProductService.Api.Requests;

public record ProductQueryRequest(string? Search, double? MaxPrice, double? MinPrice, bool? IsAvailable,
    int? UserId, string? SortBy, string? Direction, int Page = 1, int PageSize = 20);