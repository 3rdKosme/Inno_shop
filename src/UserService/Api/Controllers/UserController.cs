using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Application.Users.Queries.GetCurrentUser;
using Inno_Shop.UserService.Api.Requests.User;
using Microsoft.AspNetCore.Authorization;
using Inno_Shop.UserService.Application.Users.Commands.SendEmailConfirmationCode;

namespace Inno_Shop.UserService.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userDto = await mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return Ok(userDto);
    }

    [HttpPut("me")]
    public async Task<ActionResult> UpdateUser([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(request.ToCommand(), cancellationToken));
    }

    [HttpPost("me/deactivate")]
    public async Task<ActionResult> DeactivateUser([FromBody] DeactivateUserRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(), cancellationToken);

        return NoContent();
    }

    [HttpPost("me/activate")]
    public async Task<ActionResult> ActivateUser([FromBody] ActivateUserRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(), cancellationToken);

        return NoContent();
    }

    [HttpPost("me/sendEmailConfirmationCode")]
    public async Task<ActionResult> SendEmailConfirmationCode(CancellationToken cancellationToken)
    {
        await mediator.Send(new SendEmailConfirmationCodeCommand(), cancellationToken);

        return NoContent();
    }

    [HttpPost("me/confirmEmail")]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(), cancellationToken);

        return NoContent();
    }
}
