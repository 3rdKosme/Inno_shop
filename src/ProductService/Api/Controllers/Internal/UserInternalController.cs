using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.ProductService.Application.Products.Commands.RecoverProduct;
using Inno_Shop.ProductService.Application.Products.Commands.SoftDeleteProduct;

namespace Inno_Shop.ProductService.Api.Controllers.Internal;

[ApiController]
[Route("internal/users/{id:int}")]
public class UserInternalController(IMediator mediator) : ControllerBase
{
    
    [HttpPost("deactivate")]
    public async Task<IActionResult> SoftDeleteProducts(int id)
    {
        await mediator.Send(new SoftDeleteProductCommand(id));
        return Ok();
    }

    [HttpPost("recover")]
    public async Task<IActionResult> RecoverProducts(int id)
    {
        await mediator.Send(new RecoverProductCommand(id));
        return Ok();
    }
}