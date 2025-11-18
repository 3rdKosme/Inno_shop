using Inno_Shop.UserService.Application.Emails.Models;

namespace Inno_Shop.UserService.Application.Emails.Templates;

public static class EmailTemplateRenderer
{
    public static (string subject, string html) Render(EmailTemplate template, object model)
    {
        return template switch
        {
            EmailTemplate.PasswordReset => (
                Subjects.PasswordReset,
                PasswordReset((PasswordResetModel)model)
            ),

            EmailTemplate.EmailConfirmation => (
                Subjects.EmailConfirmation,
                EmailConfirmation((EmailConfirmationModel)model)
            ),

            EmailTemplate.ProfileChangedUser => (
                Subjects.ProfileChangedUser,
                ProfileChangedUser((ProfileChangedModel)model)
            ),

            EmailTemplate.ProfileChangedAdmin => (
                Subjects.ProfileChangedAdmin,
                ProfileChangedAdmin((ProfileChangedModel)model)
            ),

            EmailTemplate.Activated => (
                Subjects.Activated,
                Status("Ваш аккаунт был успешно активирован.")
            ),

            EmailTemplate.Deactivated => (
                Subjects.Deactivated,
                Status("Ваш аккаунт был деактивирован.")
            ),

            EmailTemplate.Locked => (
                Subjects.Locked,
                Status("Ваш аккаунт был заблокирован.")
            ),

            EmailTemplate.Unlocked => (
                Subjects.Unlocked,
                Status("Ваш аккаунт был разблокирован.")
            ),

            EmailTemplate.ProfileCreated => (
                Subjects.ProfileCreated,
                ProfileCreated((ProfileCreatedModel)model)
            ),

            _ => ("Уведомление", Layout("<p>Шаблон не найден.</p>"))
        };
    }

    private static string Layout(string body) =>
$@"<html>
<body style=""font-family:Arial;padding:20px;line-height:1.6;font-size:15px;color:#333;"">
    {body}
</body>
</html>";

    private static string Button(string url, string text) =>
$@"<a href=""{url}"" 
    style=""padding:12px 20px;background:#007bff;color:white;
           text-decoration:none;border-radius:6px;display:inline-block;margin-top:10px;"">
        {text}
</a>";

    private static string PasswordReset(PasswordResetModel m) =>
        Layout($@"
            <h2>Сброс пароля</h2>
            <p>Чтобы сбросить пароль, нажмите на кнопку ниже:</p>
            {Button(m.ResetLink, "Сбросить пароль")}
        ");

    private static string EmailConfirmation(EmailConfirmationModel m) =>
        Layout($@"
            <h2>Подтверждение email</h2>
            <p>Для подтверждения вашего адреса электронной почты нажмите кнопку:</p>
            {Button(m.ConfirmationLink, "Подтвердить email")}
        ");

    private static string ProfileChangedUser(ProfileChangedModel m) =>
        Layout($@"
            <h2>Ваш профиль был обновлён</h2>
            <p>Здравствуйте, {m.Name}. Ваш профиль был успешно изменён.</p>
        ");

    private static string ProfileChangedAdmin(ProfileChangedModel m) =>
        Layout($@"
            <h2>Профиль пользователя обновлён</h2>
            <p>Пользователь {m.Name} обновил данные своего профиля.</p>
        ");

    private static string Status(string message) =>
        Layout($@"<p>{message}</p>");

    private static string ProfileCreated(ProfileCreatedModel m) =>
        Layout($@"
            <h2>Добро пожаловать, {m.Name}!</h2>
            <p>Ваш профиль был успешно создан в системе InnoShop.</p>
            <p>Мы рады видеть вас среди наших пользователей!</p>
        ");
    
    private static class Subjects
    {
        public const string PasswordReset = "Сброс пароля";
        public const string EmailConfirmation = "Подтверждение email";
        public const string ProfileChangedUser = "Ваш профиль обновлён";
        public const string ProfileChangedAdmin = "Профиль пользователя обновлён";
        public const string Activated = "Аккаунт активирован";
        public const string Deactivated = "Аккаунт деактивирован";
        public const string Locked = "Аккаунт заблокирован";
        public const string Unlocked = "Аккаунт разблокирован";
        public const string ProfileCreated = "Добро пожаловать в InnoShop!";
    }
}
