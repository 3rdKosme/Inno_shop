namespace Inno_Shop.UserService.Application.Exceptions;

public class EmailAlreadyExistsException(string message) : Exception(message)
{
    public int statusCode = 409;
}