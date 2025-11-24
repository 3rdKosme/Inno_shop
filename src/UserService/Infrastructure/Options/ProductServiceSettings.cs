namespace Inno_Shop.UserService.Infrastructure.Options;

public class ProductServiceSettings
{
    public required string BaseAddress { get; init; }
    public required string HeaderName { get; init; }
    public required string ServiceKey { get; init; }
}