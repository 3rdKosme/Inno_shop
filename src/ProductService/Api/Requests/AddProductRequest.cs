using System.Security.Cryptography.X509Certificates;
using Inno_Shop.ProductService.Application.Products.Commands.AddProduct;
using Inno_Shop.ProductService.Application.Products.Queries.GetProducts;

namespace Inno_Shop.ProductService.Api.Requests;

public record AddProductRequest(
    string Name,
    string Description,
    double Price,
    int UserId)
{
    public AddProductCommand ToCommand() => new AddProductCommand(Name, Description, Price, UserId);
}