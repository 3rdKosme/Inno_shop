using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(int Id, string? Name, string? Description, bool? IsAvailable, double? Price) :  IRequest<Unit>;