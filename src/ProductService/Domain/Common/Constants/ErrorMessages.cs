namespace Inno_Shop.ProductService.Domain.Common.Constants;

public static class ErrorMessages
{
    public const string AlreadyActivated = "Объект уже активирован.";
    public const string AlreadyDeactivated = "Объект уже деактивирован.";
    public const string AlreadyDeleted = "Объект уже удалён.";
    public const string AlreadyRecovered = "Объект уже восстановлен.";

    public static string DomainArgumentNull(string argumentName) =>
        $"Аргумент '{argumentName}' не может быть null.";

    public static string DomainArgumentNegative(string argumentName) =>
        $"Аргумент '{argumentName}' не может быть отрицательным.";
}