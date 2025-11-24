using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Common.Constants;
using Inno_Shop.ProductService.Application.Common.Exceptions;
using Inno_Shop.ProductService.Domain.Common.Exceptions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.Shared.Application.Common.Constants;
using Inno_Shop.Shared.Application.Abstractions;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(ICurrentUserService currentUserService, IProductRepository productRepository) 
    : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId 
                     ?? throw new UnauthorizedAccessException(Shared.Application.Common.Constants.ErrorMessages.UserNotFound);
        
        var product = await productRepository.GetProductByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(ProductService.Application.Common.Constants.ErrorMessages.ProductNotFound);

        if (product.UserId != userId) throw new UnauthorizedAccessException();
        
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
        
        if (request.IsAvailable is { } available)
        {
            try
            {
                if (available) product.SetAvailable();
                else product.SetUnavailable();
            }
            catch (AlreadyDoneException ex)
            {
                throw new BusinessRuleValidationException(ex.Message);
            }
        }

        if (request.Price is { } price)
        {
            try
            {
                product.ChangePrice(price);
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