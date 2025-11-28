using Inno_Shop.ProductService.Application.Products.Commands.UpdateProduct;

namespace Inno_Shop.ProductService.Api.Requests.Management;

public record UpdateProductRequest(
    int Id,
    string? Name,
    string? Description,
    bool? IsAvailable,
    double? Price)
{
    public UpdateProductCommand ToCommand() => new (Id, Name: Name,Description: Description,
        IsAvailable: IsAvailable, Price: Price);
}