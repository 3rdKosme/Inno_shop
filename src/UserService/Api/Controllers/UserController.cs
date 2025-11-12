using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Application.Users.Queries.GetUserById;
using Inno_Shop.UserService.Application.Users.Commands.AddUser;
using Inno_Shop.UserService.Api.DTOs;
using Inno_Shop.UserService.Application.Users.Commands.UpdateUser;
using Inno_Shop.UserService.Application.Users.Commands.DeactivateUser;
using Inno_Shop.UserService.Application.Users.Commands.ActivateUser;

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

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateUser(int id, UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(
                Id: id,
                Password: request.Password,
                Name: request.Name,
                Email: request.Email,
                NewPassword: request.NewPassword
            );

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeactivateUser(int id, DeactivateUserRequest request)
    {
        var command = new DeactivateUserCommand(
                id,
                request.Password
            );

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("{id:int}")]
    public async Task<ActionResult> ActivateUser(int id, ActivateUserRequest request)
    {
        var command = new ActivateUserCommand(
                id,
                request.Password
            );

        await _mediator.Send(command);

        return NoContent();
    }
}