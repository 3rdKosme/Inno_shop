using System.Diagnostics.Contracts;

namespace Inno_Shop.UserService.Application.Common.Constants;

public static class ErrorMessages
{
    public static string IdGreaterThan0 = "a1";
    public static string NameIsRequired = "a2";
    public static string NameMustNotExceed = "a3";
    public static string EmailIsRequired = "a4";
    public static string IncorrectEmailFormat = "a5";
    public static string PasswordIsRequired = "a6";
    public static string PasswordMustBeAtLeast = "a7";
    public static string EmailAlreadyExists = "a8";
    public static string IncorrectPassword = "a9";
    public static string CurrentPasswordIsRequired = "a10";
    public static string RefreshTokenIsRequired = "a11";
    public static string TokenIsRequired = "a13";
    public static string IncorrectToken = "a14";
    public static string TokenIsExpiredOrRevoked = "a15";
    public static string UserNotFound = "a12";
}
