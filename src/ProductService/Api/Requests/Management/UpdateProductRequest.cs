using System.Security.Cryptography.X509Certificates;
using Inno_Shop.ProductService.Application.Products.Commands.UpdateProduct;
using Inno_Shop.ProductService.Application.Products.Queries.GetProducts;

namespace Inno_Shop.ProductService.Api.Requests.Management;

public record UpdateProductRequest(
    int Id,
    string? Name,
    string? Description,
    bool? IsAvailable,
    double? Price)
{
    public UpdateProductCommand ToCommand() => new UpdateProductCommand(Id, Name: Name,Description: Description,
        IsAvailable: IsAvailable, Price: Price);
}