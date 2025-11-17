using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;

namespace Inno_Shop.UserService.Application.Emails.Templates;

public static class EmailTemplateRenderer
{
    public static (string subject, string html) Render(EmailTemplate template, object model)
    {
        return template switch
        {
            EmailTemplate.PasswordReset => (
                "Reset Your Password",
                PasswordReset((PasswordResetModel)model)
            ),

            EmailTemplate.EmailConfirmation => (
                "Confirm Your Email",
                EmailConfirmation((EmailConfirmationModel)model)
            ),

            EmailTemplate.ProfileChangedUser => (
                "Your Profile Was Updated",
                ProfileChangedUser((ProfileChangedModel)model)
            ),

            EmailTemplate.ProfileChangedAdmin => (
                "A User Profile Was Updated",
                ProfileChangedAdmin((ProfileChangedModel)model)
            ),

            EmailTemplate.Activated => (
                "Account Activated",
                Status("Your account has been activated.", model)
            ),

            EmailTemplate.Deactivated => (
                "Account Deactivated",
                Status("Your account has been deactivated.", model)
            ),

            EmailTemplate.Locked => (
                "Account Locked",
                Status("Your account has been locked.", model)
            ),

            EmailTemplate.Unlocked => (
                "Account Unlocked",
                Status("Your account has been unlocked.", model)
            ),

            EmailTemplate.ProfileCreated => (
                "Welcome to InnoShop!",
                ProfileCreated((ProfileCreatedModel)model)
            ),

            _ => ("Notification", "<p>No template found</p>")
        };
    }

    private static string PasswordReset(PasswordResetModel m) =>
$@"<html><body style=""font-family:Arial;padding:20px;"">
    <h2>Password Reset</h2>
    <p>Click the link below to reset your password:</p>
    <a href=""{m.ResetLink}"" style=""padding:10px 20px;background:#007bff;color:white;text-decoration:none;border-radius:6px;"">
        Reset Password
    </a>
</body></html>";

    private static string EmailConfirmation(EmailConfirmationModel m) =>
$@"<html><body style=""font-family:Arial;padding:20px;"">
    <h2>Confirm Your Email</h2>
    <a href=""{m.ConfirmationLink}"" style=""padding:10px 20px;background:#28a745;color:white;text-decoration:none;border-radius:6px;"">
        Confirm Email
    </a>
</body></html>";

    private static string ProfileChangedUser(ProfileChangedModel m) =>
$@"<html><body style=""font-family:Arial;padding:20px;"">
    <h2>Profile Updated</h2>
    <p>Hello {m.Name}, your profile has been updated.</p>
</body></html>";

    private static string ProfileChangedAdmin(ProfileChangedModel m) =>
$@"<html><body style=""font-family:Arial;padding:20px;"">
    <h2>User Profile Updated</h2>
    <p>User {m.Name} updated their profile.</p>
</body></html>";

    private static string Status(string message, object model) =>
$@"<html><body style=""font-family:Arial;padding:20px;"">
    <p>{message}</p>
</body></html>";

    private static string ProfileCreated(ProfileCreatedModel m) =>
$@"<html><body style=""font-family:Arial;padding:20px;"">
    <h2>Добро пожаловать, {m.Name}!</h2>
    <p>Ваш профиль был успешно создан в InnoShop.</p>
    <p>Мы рады видеть вас с нами!</p>
</body></html>";
}
