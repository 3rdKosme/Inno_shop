using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Domain.Entities;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.AddProduct;

public class AddProductCommandHandler(IProductRepository productRepository) : IRequestHandler<AddProductCommand, Unit>
{
    public async Task<Unit> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.Description, request.UserId, request.Price);
        await productRepository.AddProductAsync(product, cancellationToken);
        return Unit.Value;
    }
}