using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using MediatR;
using Inno_Shop.Shared.Application.Common.Constants;

namespace Inno_Shop.ProductService.Application.Products.Commands.SoftDeleteProduct;

public class SoftDeleteProductCommandHandler(ICurrentUserService currentUserService, 
    IProductRepository productRepository) : IRequestHandler<SoftDeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllByUserIdAsync(request.Id, cancellationToken);

        foreach (var product in products)
        {
            product.Delete();
            await productRepository.UpdateProductAsync(product, cancellationToken);
        }
        
        return Unit.Value;
    }
}
