namespace Inno_Shop.UserService.Domain.Common.Exceptions;

public class EmailAlreadyConfirmedException : Exception
{
    public EmailAlreadyConfirmedException() : base("Email Already confirmed.") { }
}