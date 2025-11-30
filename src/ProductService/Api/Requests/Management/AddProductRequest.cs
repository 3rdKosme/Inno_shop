using Inno_Shop.ProductService.Application.Products.Commands.AddProduct;

namespace Inno_Shop.ProductService.Api.Requests.Management;

public record AddProductRequest(
    string Name,
    string Description,
    double Price)
{
    public AddProductCommand ToCommand() => new (Name, Description, Price);
}