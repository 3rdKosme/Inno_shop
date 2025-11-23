using Inno_Shop.ProductService.Domain.Common.Constants;
using Inno_Shop.ProductService.Domain.Common.Exceptions;
using System.Numerics;

namespace Inno_Shop.ProductService.Domain.Common;

public static class Guard
{
    public static void AgainstNullOrWhiteSpace(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainArgumentException(string.Format(ErrorMessages.DomainArgumentNull(parameterName)));
        }
    }
    
    public static void AgainstNullOrNegative<T>(T? value, string parameterName)
        where T : struct, INumber<T>
    {
        if (value is null)
            throw new DomainArgumentException(ErrorMessages.DomainArgumentNull(parameterName));
        if (value < T.Zero)
            throw new DomainArgumentException(ErrorMessages.DomainArgumentNegative(parameterName));
    }
}