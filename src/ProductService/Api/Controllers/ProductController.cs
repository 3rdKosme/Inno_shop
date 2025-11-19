using Microsoft.AspNetCore.Mvc;
using MediatR

namespace Inno_Shop.ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public async Task GetAllProducts()
    {

    }
}