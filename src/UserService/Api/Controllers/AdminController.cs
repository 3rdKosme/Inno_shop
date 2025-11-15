using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Application.Users.Queries.GetUserById;
using Inno_Shop.UserService.Application.Users.Commands.AddUser;
using Inno_Shop.UserService.Api.DTOs;
using Inno_Shop.UserService.Application.Users.Commands.UpdateUser;
using Inno_Shop.UserService.Application.Users.Commands.DeactivateUser;
using Inno_Shop.UserService.Application.Users.Commands.ActivateUser;
using Microsoft.AspNetCore.Authorization;
using Inno_Shop.UserService.Application.Abstractions;
using System.Security.Claims;
using Inno_Shop.UserService.Application.Users.Queries.GetUserByIdAdmin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{int:id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var query = new GetUserByIdAdminQuery(id);
        var userDto = await _mediator.Send(query);
        return Ok(userDto);
    }

    [HttpPut("{int:id}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(
                Password: request.Password,
                Name: request.Name,
                Email: request.Email,
                NewPassword: request.NewPassword
            );

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("me")]
    public async Task<ActionResult> DeactivateUser([FromBody] DeactivateUserRequest request)
    {
        var command = new DeactivateUserCommand(request.Password);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("me/activate")]
    public async Task<ActionResult> ActivateUser([FromBody] ActivateUserRequest request)
    {
        var command = new ActivateUserCommand(request.Password);

        await _mediator.Send(command);

        return NoContent();
    }
}
