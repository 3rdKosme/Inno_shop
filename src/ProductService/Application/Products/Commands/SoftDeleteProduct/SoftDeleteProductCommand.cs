using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.SoftDeleteProduct;

public sealed record SoftDeleteProductCommand(int Id) : IRequest<Unit>;