using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.ProductService.Api.Requests.Management;

namespace Inno_Shop.ProductService.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ManagementController(IMediator mediator) : ControllerBase
{
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