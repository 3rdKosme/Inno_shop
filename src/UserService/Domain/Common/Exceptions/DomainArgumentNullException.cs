namespace Inno_Shop.UserService.Domain.Common.Exceptions;

public class DomainArgumentNullException : Exception
{
    public DomainArgumentNullException(string message) : base(message) { }
}