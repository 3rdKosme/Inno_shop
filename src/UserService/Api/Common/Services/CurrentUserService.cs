using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Inno_Shop.UserService.Api.Common.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public int UserId => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst("sub")!.Value);
}