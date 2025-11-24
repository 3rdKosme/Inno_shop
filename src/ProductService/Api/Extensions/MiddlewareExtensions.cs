using Inno_Shop.ProductService.Api.Middleware;

namespace Inno_Shop.ProductService.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<InternalAuthMiddleware>();
        return app;
    }
}