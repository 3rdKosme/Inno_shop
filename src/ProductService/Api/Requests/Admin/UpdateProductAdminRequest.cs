using Inno_Shop.ProductService.Application.Products.Commands.UpdateProductAdmin;

namespace Inno_Shop.ProductService.Api.Requests.Admin;

public record UpdateProductAdminRequest(string? Name, string? Description)
{
    public UpdateProductAdminCommand ToCommand(int id) =>
        new (Id: id, Name: Name, Description: Description);
}