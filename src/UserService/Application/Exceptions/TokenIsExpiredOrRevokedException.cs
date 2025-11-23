namespace Inno_Shop.UserService.Application.Exceptions;

public class TokenIsExpiredOrRevokedException(string message) : Exception(message) { }