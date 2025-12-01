using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.RecoverProduct;

public sealed record RecoverProductCommand(int Id) : IRequest<Unit>;