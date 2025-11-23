using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Common.Constants;
using Inno_Shop.Shared.Application.Exceptions;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductRepository productRepository) : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetProductByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.ProductNotFound);
        if (request.Name is not null)
        {
            product.ChangeName(request.Name);
        }

        if (request.Description is not null)
        {
            product.ChangeDescription(request.Description);
        }

        if (request.Price is { } price)
        {
            product.ChangePrice(price);
        }

        await productRepository.UpdateProductAsync(product, cancellationToken);
        
        return Unit.Value;
    }
}