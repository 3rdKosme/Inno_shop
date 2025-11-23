namespace Inno_Shop.UserService.Application.Common.Constants;

public static class ErrorMessages
{
    public const string IdMustBeGreaterThan0 = "Идентификатор должен быть больше нуля.";
    public const string NameIsRequired = "Имя является обязательным для заполнения.";
    public const string NameMustNotExceed = "Длина имени превышает допустимое значение.";
    public const string EmailIsRequired = "Адрес электронной почты является обязательным для заполнения.";
    public const string IncorrectEmailFormat = "Указан некорректный формат адреса электронной почты.";
    public const string PasswordIsRequired = "Пароль является обязательным для заполнения.";
    public const string PasswordMustBeAtLeast = "Пароль не соответствует минимальному требованию по длине.";
    public const string EmailAlreadyExists = "Указанный адрес электронной почты уже зарегистрирован в системе.";
    public const string IncorrectPassword = "Введён неверный Email или пароль.";
    public const string CurrentPasswordIsRequired = "Текущий пароль является обязательным для заполнения.";
    public const string RefreshTokenIsRequired = "Необходимо предоставить refresh-токен.";
    public const string TokenIsRequired = "Необходимо предоставить токен.";
    public const string IncorrectToken = "Указан некорректный токен.";
    public const string TokenIsExpiredOrRevoked = "Токен просрочен или был отозван.";
    public const string UserNotFound = "Пользователь с указанными данными не найден.";
}