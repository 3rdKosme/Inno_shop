using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Application.Users.Queries.GetUserByIdAdmin;
using Inno_Shop.UserService.Application.Users.Commands.AddUser;
using Inno_Shop.UserService.Api.DTOs;
using Inno_Shop.UserService.Application.Users.Commands.UpdateUser;
using Inno_Shop.UserService.Application.Users.Commands.DeactivateUser;
using Inno_Shop.UserService.Application.Users.Commands.ActivateUser;
using Microsoft.AspNetCore.Authorization;
using Inno_Shop.UserService.Application.Abstractions;
using System.Security.Claims;
using Inno_Shop.UserService.Application.Users.Commands.LockUser;
using Inno_Shop.UserService.Application.Users.Commands.UnlockUser;
using Inno_Shop.UserService.Application.Users.Commands.UpdateUserAdmin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var query = new GetUserByIdAdminQuery(id);
        var userDto = await _mediator.Send(query);
        return Ok(userDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserAdminRequest request)
    {
        var command = new UpdateUserAdminCommand(
                Id: id,
                Name: request.Name
            );

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("{id:int}/lock")]
    public async Task<ActionResult> LockUser(int id)
    {
        var command = new LockUserCommand(id);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("{id:int}/unlock")]
    public async Task<ActionResult> UnlockUser(int id)
    {
        var command = new UnlockUserCommand(id);

        await _mediator.Send(command);

        return NoContent();
    }
}
