using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Inno_Shop.ProductService.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Inno_Shop.ProductService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    private const string ConnectionStringSectionName = "DefaultConnection";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(ConnectionStringSectionName)));
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}
