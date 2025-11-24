using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Application.Common.Constants;
using Inno_Shop.ProductService.Application.DTOs;
using Inno_Shop.Shared.Application.Exceptions;
using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetProductByIdAsync(request.Id, cancellationToken) 
            ?? throw new NotFoundException(ErrorMessages.ProductNotFound);

        return new ProductDto(
            Id: product.Id,
            Name: product.Name,
            Description: product.Description,
            Price: product.Price,
            IsAvailable: product.IsAvailable,
            UserId: product.UserId,
            CreatedAt: product.CreatedAt);
    }
}