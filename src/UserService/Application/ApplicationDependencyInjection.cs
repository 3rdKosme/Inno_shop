using Inno_Shop.UserService.Application.Common.Behaviors;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Inno_Shop.UserService.Application.Common.Settings;

namespace Inno_Shop.UserService.Application;

public static class ApplicationDependencyInjection
{
    private const string AppSettingsSectionName = "AppSettings";
    private const string EmailConfirmationSectionName = "EmailConfirmationTokenSettings";
    private const string PasswordResetSectionName = "PasswordResetTokenSettings";
    private const string RefreshSectionName = "RefreshTokenSettings";
    private const string TokenCleanupPolicySectionName = "TokenCleanupPolicy";
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(ApplicationDependencyInjection).Assembly;
        services.Configure<AppSettings>(configuration.GetSection(AppSettingsSectionName));
        services.Configure<EmailConfirmationTokenSettings>(configuration.GetSection(EmailConfirmationSectionName));
        services.Configure<PasswordResetTokenSettings>(configuration.GetSection(PasswordResetSectionName));
        services.Configure<RefreshTokenSettings>(configuration.GetSection(RefreshSectionName));
        services.Configure<TokenCleanupPolicy>(configuration.GetSection(TokenCleanupPolicySectionName));

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));
        
        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
