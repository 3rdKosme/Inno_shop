namespace Inno_Shop.UserService.Application.DTOs;

public record UserDto(int Id, string Name, string Email, string Role, bool IsEmailConfirmed, bool IsActive);