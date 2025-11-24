using MediatR;

namespace Inno_Shop.ProductService.Application.Products.Commands.UpdateProductAdmin;

public sealed record UpdateProductAdminCommand(int Id, string? Name, string? Description) : IRequest<Unit>;