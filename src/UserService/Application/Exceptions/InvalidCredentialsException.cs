namespace Inno_Shop.UserService.Application.Exceptions;

public class InvalidCredentialsException(string message) : Exception(message)
{
    public int statusCode = 401;
}