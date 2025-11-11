using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Application.Users.Queries.GetUserById;

[ApiController]
[Route("api/[controller]")]
public class UserController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var query = new GetUserByIdQuery(id);
        var userDto = await _mediator.Send(query);
        return Ok(userDto);
    }
}