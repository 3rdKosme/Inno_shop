using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Api.DTOs;
using Inno_Shop.UserService.Application.Users.Commands.LoginUser;
using Inno_Shop.UserService.Application.Users.Commands.RefreshToken;
using Inno_Shop.UserService.Application.Users.Commands.AddUser;
using Inno_Shop.UserService.Application.Users.Commands.SendPasswordResetCode;
using Microsoft.AspNetCore.Authorization;
using Inno_Shop.UserService.Application.Users.Commands.ResetPassword;

namespace Inno_Shop.UserService.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var authResult = await _mediator.Send(command);

        return Ok(authResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.Token);

        var authResult = await _mediator.Send(command);

        return Ok(authResult);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AddUserRequest request)
    {
        var command = new AddUserCommand(
                request.Name,
                request.Email,
                request.Password
            );

        var authResult = await _mediator.Send(command);

        return Ok(authResult);
    }

    [HttpPost("forgotPassword")]
    public async Task<ActionResult> SendPasswordResetCode([FromBody] SendPasswordResetCodeRequest request)
    {
        var command = new SendPasswordResetCodeCommand(request.Email);

        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("validateResetToken")]
    public async Task<ActionResult> ValidateResetToken([FromBody] ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(request.Token, request.NewPassword);

        await _mediator.Send(command);

        return Ok();
    }
}
