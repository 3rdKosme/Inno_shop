using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Inno_Shop.UserService.Infrastructure.Services;
using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Inno_Shop.UserService.Infrastructure.Options;

namespace Inno_Shop.UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.Configure<TokenGeneratorSettings>(configuration.GetSection("TokenGeneratorSettings"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IEmailConfirmationTokenRepository, EmailConfirmationTokenTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        return services;
    }
}
