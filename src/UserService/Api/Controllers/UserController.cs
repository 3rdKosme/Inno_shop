using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Application.Users.Queries.GetCurrentUser;
using Inno_Shop.UserService.Application.Users.Commands.AddUser;
using Inno_Shop.UserService.Api.DTOs;
using Inno_Shop.UserService.Application.Users.Commands.UpdateUser;
using Inno_Shop.UserService.Application.Users.Commands.DeactivateUser;
using Inno_Shop.UserService.Application.Users.Commands.ActivateUser;
using Microsoft.AspNetCore.Authorization;
using Inno_Shop.UserService.Application.Abstractions;
using System.Security.Claims;
using Inno_Shop.UserService.Application.Users.Commands.SendEmailConfirmationCode;
using Inno_Shop.UserService.Application.Users.Commands.ConfirmEmail;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var query = new GetCurrentUserQuery();
        var userDto = await _mediator.Send(query);
        return Ok(userDto);
    }

    [HttpPut("me")]
    public async Task<ActionResult> UpdateUser([FromBody] UpdateUserRequest request)
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

    [HttpPost("me/deactivate")]
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

    [HttpPost("me/sendEmailConformationCode")]
    public async Task<ActionResult> SendEmailConfirmationCode()
    {
        var command = new SendEmailConfirmationCodeCommand();
        
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("me/confirmEmail")]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var command = new ConfirmEmailCommand(request.Token);

        await _mediator.Send(command);

        return NoContent();
    }
}
