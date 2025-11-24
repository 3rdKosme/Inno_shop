using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.RecoverProduct;

public record RecoverProductCommand(int Id) : IRequest<Unit>;