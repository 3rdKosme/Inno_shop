namespace Inno_Shop.ProductService.Application.Common.Constants;

public static class ErrorMessages
{
    public const string MinPriceGreaterThanMax = "Минимальная цена не может быть больше максимальной.";
    public const string InvalidSortType = "Недопустимый тип сортировки.";
    public const string QueryIsTooLong = "Поисковый запрос слишком длинный.";
    public const string PageMustBePositive = "Номер страницы должен быть положительным числом.";
    public const string PageSizeMustBeBetween = "Размер страницы должен быть от 1 до 100.";
    public const string NameIsRequired = "Название обязательно для заполнения.";
    public const string NameMustNotExceed = "Название не должно превышать 100 символов.";
    public const string DescriptionIsRequired = "Описание обязательно для заполнения.";
    public const string DescriptionMustNotExceed = "Описание не должно превышать 500 символов.";
    public const string PriceIsRequired = "Цена обязательна для указания.";
    public const string PriceMustBePositive = "Цена должна быть положительным числом.";
    public const string IdIsRequired = "Идентификатор обязателен.";
    public const string IdMustBePositive = "Идентификатор должен быть положительным числом.";
    public const string ProductNotFound = "Товар не найден.";
}