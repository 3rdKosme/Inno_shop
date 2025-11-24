using Inno_Shop.ProductService.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Inno_Shop.ProductService.Api.Middleware;

public class InternalAuthMiddleware(IOptions<ProductServiceOptions> productServiceOptions, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? throw new Exception("Path is empty");

        if (path.StartsWith("/internal"))
        {
            if (!context.Request.Headers.TryGetValue(productServiceOptions.Value.HeaderName, out var key) ||
                key != productServiceOptions.Value.ServiceKey)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid service key.");
                return;
            }
        }
        
        await next(context);
    }
}
