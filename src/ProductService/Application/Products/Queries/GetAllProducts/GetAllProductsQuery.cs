using MediatR;
using Inno_Shop.ProductService.Domain.Entities;
using System.Net;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetAllProducts;

public sealed record GetAllProductsQuery() : IRequest<IEnumerable<Product>>;