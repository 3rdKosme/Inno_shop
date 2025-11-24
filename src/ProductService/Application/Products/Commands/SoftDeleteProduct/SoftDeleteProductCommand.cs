using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.SoftDeleteProduct;

public record SoftDeleteProductCommand() : IRequest<Unit>;