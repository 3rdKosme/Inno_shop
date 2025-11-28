namespace Inno_Shop.UserService.Domain.Common.Constants;

public static class ErrorMessages
{
    public const string AlreadyActivated = "Указанный пользователь уже активирован и не требует повторного изменения статуса.";
    public const string AlreadyDeactivated = "Указанный пользователь уже деактивирован и не требует повторного изменения статуса.";
    public const string AlreadyLocked = "Учетная запись пользователя уже заблокирована.";
    public const string AlreadyUnlocked = "Учетная запись пользователя уже разблокирована.";
    public const string EmailAlreadyConfirmed = "Адрес электронной почты уже подтверждён и не требует повторной верификации.";
    public const string AlreadyPromoted =
        "Указанный пользователь уже является администратором и не требует повторного повышения.";
    public static string DomainArgumentNull(string argumentName) =>
        $"Переданный аргумент доменной модели '{argumentName}' содержит недопустимое значение null.";
}
