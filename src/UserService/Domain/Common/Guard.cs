using Inno_Shop.UserService.Domain.Common.Constants;
using Inno_Shop.UserService.Domain.Common.Exceptions;

namespace Inno_Shop.UserService.Domain.Common;

public static class Guard
{
    public static void AgainstNull<T>(T value, string parameterName)
    {
        if (value == null) 
        {
            throw new DomainArgumentNullException(string.Format(ErrorMessages.DomainArgumentNull(parameterName)));
        }
    }
    public static void AgainstNullOrWhiteSpace(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainArgumentNullException(string.Format(ErrorMessages.DomainArgumentNull(parameterName)));
        }
    }
}