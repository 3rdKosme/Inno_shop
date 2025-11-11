namespace Inno_Shop.UserService.Api.DTOs;

public record UpdateUserRequest(int Id, string Password, string? Name, string? Email, string? NewPassword);