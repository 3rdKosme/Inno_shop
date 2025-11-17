using Inno_Shop.UserService.Application.Common.Behaviors;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Inno_Shop.UserService.Application.Common.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Inno_Shop.UserService.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(ApplicationServiceRegistration).Assembly;
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<EmailConfirmationTokenSettings>(configuration.GetSection("EmailConfirmationTokenSettings"));
        services.Configure<PasswordResetTokenSettings>(configuration.GetSection("PasswordResetTokenSettings"));
        services.Configure<RefreshTokenSettings>(configuration.GetSection("RefreshTokenSettings"));
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));
        

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
