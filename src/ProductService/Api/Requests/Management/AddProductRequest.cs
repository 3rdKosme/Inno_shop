using System.Security.Cryptography.X509Certificates;
using Inno_Shop.ProductService.Application.Products.Commands.AddProduct;
using Inno_Shop.ProductService.Application.Products.Queries.GetProducts;

namespace Inno_Shop.ProductService.Api.Requests.Management;

public record AddProductRequest(
    string Name,
    string Description,
    double Price)
{
    public AddProductCommand ToCommand() => new AddProductCommand(Name, Description, Price);
}