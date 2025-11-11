namespace Inno_Shop.Shared.Application.Exceptions;

public class NotFoundException : Exception
{
    int statusCode;
    public NotFoundException(string message) : base(message)
    {
        statusCode = 404;
    }
}