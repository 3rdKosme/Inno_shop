using Inno_Shop.ProductService.Application.Abstractions;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.RecoverProduct;

public class RecoverProductCommandHandler(IProductRepository productRepository) 
    : IRequestHandler<RecoverProductCommand, Unit>
{
    public async Task<Unit> Handle(RecoverProductCommand request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllByUserIdAsync(request.Id, cancellationToken);

        foreach (var product in products)
        {
            product.Recover();
            await productRepository.UpdateProductAsync(product, cancellationToken);
        }
        
        return Unit.Value;
    }
}
