using Inno_Shop.UserService.Application.Abstractions;

namespace Inno_Shop.UserService.Infrastructure.Clients;

public class ProductServiceClient(HttpClient httpClient) : IProductServiceClient
{
    public async Task DeactivateProductsAsync(int userId, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"/internal/users/{userId}/deactivate", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task RecoverProductsAsync(int userId, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"/internal/users/{userId}/recover", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}