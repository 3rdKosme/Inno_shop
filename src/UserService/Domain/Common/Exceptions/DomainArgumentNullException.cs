namespace Inno_Shop.UserService.Domain.Common.Exceptions;

public class DomainArgumentNullException : Exception
{
    public DomainArgumentNullException(string parameterName) : base($"{parameterName} is required.") { }
}