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

        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
