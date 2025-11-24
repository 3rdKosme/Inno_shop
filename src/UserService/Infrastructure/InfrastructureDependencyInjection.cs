using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Inno_Shop.UserService.Infrastructure.Services;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Infrastructure.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Inno_Shop.Shared.Infrastructure.Services;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.UserService.Infrastructure.Clients;
using Inno_Shop.UserService.Infrastructure.Options;

namespace Inno_Shop.UserService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    private const string ConnectionStringSectionName = "DefaultConnection";
    private const string SmtpSettingsSectionName = "SmtpSettings";
    private const string TokenGeneratorSettingsSectionName = "TokenGeneratorSettings";
    private const string ProductServiceSettingsSectionName = "ProductServiceSettings";
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(ConnectionStringSectionName)));
        
        var productServiceSettings = configuration.GetSection(ProductServiceSettingsSectionName).Get<ProductServiceSettings>() 
                                     ?? throw new Exception("Product service settings not configured");

        services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
        {
            client.BaseAddress = new Uri(productServiceSettings.BaseAddress);
            client.DefaultRequestHeaders.Add(productServiceSettings.HeaderName, productServiceSettings.ServiceKey);
        });

        
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettingsSectionName));
        services.Configure<TokenGeneratorSettings>(configuration.GetSection(TokenGeneratorSettingsSectionName));
        services.Configure<ProductServiceSettings>(configuration.GetSection(ProductServiceSettingsSectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenRepository<RefreshToken>, RefreshTokenRepository>();
        services.AddScoped<ITokenRepository<EmailConfirmationToken>, EmailConfirmationTokenTokenRepository>();
        services.AddScoped<ITokenRepository<PasswordResetToken>, PasswordResetTokenRepository>();
        services.AddScoped<ITokenRepository<BaseToken>, TokenRepository<BaseToken>>();
        
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ITokenCleanupService, TokenCleanupService>();

        services.AddHostedService<TokenCleanupHostService>();
        return services;
    }
}
