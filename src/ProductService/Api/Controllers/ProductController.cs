using Microsoft.AspNetCore.Mvc;
using MediatR;
using Inno_Shop.ProductService.Api.Requests;

namespace Inno_Shop.ProductService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryRequest request)
    {

    }
}