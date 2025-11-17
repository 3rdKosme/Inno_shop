namespace Inno_Shop.UserService.Domain.Common.Exceptions;

public class AlreadyDoneException : Exception
{
    public AlreadyDoneException() : base("Email Already confirmed.") { }
}