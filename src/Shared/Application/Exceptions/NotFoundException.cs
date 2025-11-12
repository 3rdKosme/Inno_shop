namespace Inno_Shop.Shared.Application.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
    public int statusCode = 404;
    
}