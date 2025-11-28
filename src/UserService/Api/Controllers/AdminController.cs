using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Application.Users.Queries.GetUserByIdAdmin;
using Inno_Shop.UserService.Api.Requests.Admin;
using Microsoft.AspNetCore.Authorization;
using Inno_Shop.UserService.Application.Users.Commands.LockUser;
using Inno_Shop.UserService.Application.Users.Commands.UnlockUser;
using Inno_Shop.UserService.Application.Users.Commands.PromoteUserToAdmin;

namespace Inno_Shop.UserService.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]/user/{id:int}")]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
    {
        var userDto = await mediator.Send(new GetUserByIdAdminQuery(id), cancellationToken);
        return Ok(userDto);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserAdminRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(id), cancellationToken);

        return NoContent();
    }

    [HttpPost("lock")]
    public async Task<ActionResult> LockUser(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new LockUserCommand(id), cancellationToken);

        return NoContent();
    }

    [HttpPost("unlock")]
    public async Task<ActionResult> UnlockUser(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new UnlockUserCommand(id), cancellationToken);

        return NoContent();
    }

    [HttpPost("promoteToAdmin")]
    public async Task<ActionResult> PromoteToAdmin(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new PromoteUserToAdminCommand(id), cancellationToken);

        return NoContent();
    }
}