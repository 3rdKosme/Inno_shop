using Microsoft.AspNetCore.Mvc;
using MediatR;
using Inno_Shop.ProductService.Api.Requests;

namespace Inno_Shop.ProductService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryRequest request, 
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(request.ToQuery(), cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult> AddProduct([FromBody] AddProductRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(), cancellationToken);
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(), cancellationToken);
        return Ok();
    }
}