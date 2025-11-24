using Inno_Shop.ProductService.Api.Requests.Admin;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Inno_Shop.ProductService.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpPut("product/{id:int}")]
    public async Task<ActionResult> UpdateProductAdmin(int id, 
        [FromBody] UpdateProductAdminRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(id), cancellationToken);
        return NoContent();
    }
}