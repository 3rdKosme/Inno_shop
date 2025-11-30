using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Common.Exceptions;
using Inno_Shop.ProductService.Domain.Common.Exceptions;
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
            try
            {
                product.Recover();
                await productRepository.UpdateProductAsync(product, cancellationToken);
            }
            catch (AlreadyDoneException ex)
            {
                throw new BusinessRuleValidationException(ex.Message);
            }
        }
        
        return Unit.Value;
    }
}
