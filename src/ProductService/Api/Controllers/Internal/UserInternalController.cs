using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.ProductService.Application.Products.Commands.RecoverProduct;
using Inno_Shop.ProductService.Application.Products.Commands.SoftDeleteProduct;

namespace Inno_Shop.ProductService.Api.Controllers.Internal;

[ApiController]
[Route("internal/users")]
public class UserInternalController(IMediator mediator) : ControllerBase
{
    
    [HttpPost("{id:int}/deactivate")]
    public async Task<IActionResult> SoftDeleteProducts(int id)
    {
        await mediator.Send(new SoftDeleteProductCommand(id));
        return Ok();
    }

    [HttpPost("{id:int}/recover")]
    public async Task<IActionResult> RecoverProducts(int id)
    {
        await mediator.Send(new RecoverProductCommand(id));
        return Ok();
    }
}