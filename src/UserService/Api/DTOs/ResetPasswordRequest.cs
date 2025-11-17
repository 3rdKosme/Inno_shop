namespace Inno_Shop.UserService.Api.DTOs;

public record ResetPasswordRequest(string Token, string NewPassword);
