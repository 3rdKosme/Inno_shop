using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Infrastructure.Options;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Inno_Shop.ProductService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    private const string ConnectionStringSectionName = "DefaultConnection";
    private const string ProductServiceOptionsSectionName = "ProductServiceOptions";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(ConnectionStringSectionName)));
        services.Configure<ProductServiceOptions>(configuration.GetSection(ProductServiceOptionsSectionName));

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}
