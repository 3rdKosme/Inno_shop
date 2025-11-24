using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.Shared.Application.Common.Constants;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.AddProduct;

public class AddProductCommandHandler(ICurrentUserService currentUserService, IProductRepository productRepository) 
    : IRequestHandler<AddProductCommand, Unit>
{
    public async Task<Unit> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException(ErrorMessages.UserNotFound);
        var product = new Product(request.Name, request.Description, userId, request.Price);
        await productRepository.AddProductAsync(product, cancellationToken);
        return Unit.Value;
    }
}