using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.AddProduct;

public sealed record AddProductCommand(string Name, string Description, double Price, int UserId) :  IRequest<Unit>;