using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Common.Exceptions;
using Inno_Shop.ProductService.Domain.Common.Exceptions;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.SoftDeleteProduct;

public class SoftDeleteProductCommandHandler(IProductRepository productRepository) 
    : IRequestHandler<SoftDeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllByUserIdAsync(request.Id, cancellationToken);

        foreach (var product in products)
        {
            try
            {
                product.Delete();
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
