namespace Inno_Shop.UserService.Domain.Common.Exceptions;

public class AlreadyDeactivatedException : Exception
{
    public AlreadyDeactivatedException() : base("Email Already confirmed.") { }
}