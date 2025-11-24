using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Common.Constants;
using Inno_Shop.ProductService.Application.Common.Exceptions;
using Inno_Shop.ProductService.Domain.Common.Exceptions;
using Inno_Shop.Shared.Application.Exceptions;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.UpdateProductAdmin;

public class UpdateProductAdminCommandHandler(IProductRepository productRepository) : IRequestHandler<UpdateProductAdminCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductAdminCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetProductByIdAsync(request.Id, cancellationToken)
            ?? throw new UnauthorizedAccessException(ErrorMessages.ProductNotFound);
        
        if (request.Name is not null)
        {
            try
            {
                product.ChangeName(request.Name);
            }
            catch (DomainArgumentException ex)
            {
                throw new BusinessRuleValidationException(ex.Message);
            }
        }

        if (request.Description is not null)
        {
            try
            {
                product.ChangeDescription(request.Description);
            }
            catch (DomainArgumentException ex)
            {
                throw new BusinessRuleValidationException(ex.Message);
            }
        }
        
        await productRepository.UpdateProductAsync(product, cancellationToken);
        
        return Unit.Value;
    }
}