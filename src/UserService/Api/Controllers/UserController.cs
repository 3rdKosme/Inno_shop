using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Application.Users.Queries.GetUserById;
using Inno_Shop.UserService.Application.Users.Commands.AddUser;
using Inno_Shop.UserService.Api.DTOs;

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

    [HttpPost]
    public async Task<IActionResult> AddUser(AddUserRequest request)
    {
        var command = new AddUserCommand(
                request.Name,
                request.Email,
                request.Password
            );

        var userId = await _mediator.Send(command);

        return Ok(new { Id = userId });
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser()
    {

    }
}