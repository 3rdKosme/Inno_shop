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

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("me")]
    public async Task<IActionResult> GetUserById()
    {
        int userId;
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId)) throw new UnauthorizedAccessException();
        
        var query = new GetUserByIdQuery(userId);
        var userDto = await _mediator.Send(query);
        return Ok(userDto);
    }

    [HttpPut("me")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
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
    public async Task<ActionResult> DeactivateUser(int id, [FromBody] DeactivateUserRequest request)
    {
        var command = new DeactivateUserCommand(
                id,
                request.Password
            );

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("{id:int}")]
    public async Task<ActionResult> ActivateUser(int id, [FromBody] ActivateUserRequest request)
    {
        var command = new ActivateUserCommand(
                id,
                request.Password
            );

        await _mediator.Send(command);

        return NoContent();
    }
}
