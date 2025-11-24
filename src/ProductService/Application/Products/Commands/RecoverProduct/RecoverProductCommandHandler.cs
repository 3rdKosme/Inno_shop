using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using MediatR;
using Inno_Shop.Shared.Application.Common.Constants;

namespace Inno_Shop.ProductService.Application.Products.Commands.RecoverProduct;

public class RecoverProductCommandHandler(ICurrentUserService currentUserService, 
    IProductRepository productRepository) : IRequestHandler<RecoverProductCommand, Unit>
{
    public async Task<Unit> Handle(RecoverProductCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException(ErrorMessages.UserNotFound);

        var products = await productRepository.GetAllByUserIdAsync(userId, cancellationToken);

        foreach (var product in products)
        {
            product.Recover();
            await productRepository.UpdateProductAsync(product, cancellationToken);
        }
        
        return Unit.Value;
    }
}
