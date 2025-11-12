namespace Inno_Shop.UserService.Domain.Common.Exceptions;

public class AlreadyActivatedException : Exception
{
    public AlreadyActivatedException() : base("Email Already confirmed.") { }
}