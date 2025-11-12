namespace Inno_Shop.UserService.Api.DTOs;

public record UpdateUserRequest(string Password, string? Name, string? Email, string? NewPassword);