using Inno_Shop.UserService.Api.Extensions;

namespace Inno_Shop.UserService.Api;

public static class ApiDependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor()
            .AddApiControllers()
            .AddSwaggerDocumentation()
            .AddJwtAuthentication(configuration);
        
        return services;
    }

    public static IApplicationBuilder UseApi(this IApplicationBuilder app)
    {
        if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            app.UseSwaggerDocumentation();
        }

        app.UseCustomExceptionHandler()
            .UseHttpsRedirection()
            .UseAuthentication()
            .UseAuthorization();

        return app;
    }
}