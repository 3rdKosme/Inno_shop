namespace Inno_Shop.UserService.Application.Abstractions;

public interface IProductServiceClient
{
    Task DeactivateProductsAsync(int userId, CancellationToken cancellationToken = default);
    Task RecoverProductsAsync(int userId, CancellationToken cancellationToken = default);
}