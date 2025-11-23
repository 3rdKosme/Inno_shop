namespace Inno_Shop.ProductService.Domain.Common.Constants;

public static class ErrorMessages
{
    public const string AlreadyActivated = "a1";
    public const string AlreadyDeactivated = "a2";
    public const string AlreadyDeleted = "a3";
    public const string AlreadyRecovered = "a4";

    public static string DomainArgumentNull(string argumentName) =>
        $"a1 {argumentName} a2";
    public static string DomainArgumentNegative(string argumentName) =>
        $"a3 {argumentName} a4";
}