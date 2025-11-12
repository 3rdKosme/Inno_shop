using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Inno_Shop.UserService.Infrastructure.Services;
using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Inno_Shop.UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
