namespace Inno_Shop.ProductService.Infrastructure.Options;

public class JwtSettings
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int ExpireMinutes { get; init; }
}