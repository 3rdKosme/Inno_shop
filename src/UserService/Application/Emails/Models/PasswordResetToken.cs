namespace Inno_Shop.UserService.Application.Emails.Models;

public class PasswordResetModel
{
    public required string ResetLink { get; init; }
}
