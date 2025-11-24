using Microsoft.AspNetCore.Mvc;
using MediatR;
using Inno_Shop.ProductService.Api.Requests.Catalog;
using Inno_Shop.ProductService.Application.Products.Queries.GetProductById;

namespace Inno_Shop.ProductService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryRequest request, 
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(request.ToQuery(), cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetProductByIdQuery(id), cancellationToken));
    }
}