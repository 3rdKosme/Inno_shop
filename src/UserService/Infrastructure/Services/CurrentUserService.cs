using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Inno_Shop.UserService.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public int? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? Role
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}