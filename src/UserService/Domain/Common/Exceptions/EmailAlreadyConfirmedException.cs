namespace Inno_Shop.UserService.Domain.Common.Exceptions;

public class EmailAlreadyConfirmedException : Exception
{
    public EmailAlreadyConfirmedException(string message) : base(message) { }
}