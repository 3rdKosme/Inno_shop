using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inno_Shop.UserService.Api.Requests.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Inno_Shop.UserService.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authResult = await mediator.Send(request.ToCommand(), cancellationToken);

        return Ok(authResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResult = await mediator.Send(request.ToCommand(), cancellationToken);

        return Ok(authResult);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AddUserRequest request, CancellationToken cancellationToken)
    {
        var authResult = await mediator.Send(request.ToCommand(), cancellationToken);

        return Ok(authResult);
    }

    [HttpPost("forgotPassword")]
    public async Task<ActionResult> SendPasswordResetCode([FromBody] SendPasswordResetCodeRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(), cancellationToken);

        return Ok();
    }

    [HttpPost("resetPassword")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request.ToCommand(), cancellationToken);

        return Ok();
    }
}
