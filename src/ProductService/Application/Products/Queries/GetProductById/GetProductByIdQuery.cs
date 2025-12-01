using MediatR;
using Inno_Shop.ProductService.Application.DTOs;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IRequest<ProductDto>;