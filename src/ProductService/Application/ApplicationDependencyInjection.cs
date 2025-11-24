using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Inno_Shop.ProductService.Application.Common.Behaviors;

namespace Inno_Shop.ProductService.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationDependencyInjection).Assembly;
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));
        
        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}