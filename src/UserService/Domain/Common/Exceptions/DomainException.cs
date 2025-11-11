namespace Inno_Shop.UserService.Domain.Common.Exceptions;

public class DomainException : Exception
{
    public DomainException(string parameterName) : base($"{parameterName} is required.") { }
}