using Inno_Shop.ProductService.Application.Common.Exceptions;
using FluentValidation;
using Inno_Shop.Shared.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Inno_Shop.ProductService.Api.Extensions;

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                var result = ex switch
                {
                    ValidationException e => Results.Problem(e.Message, statusCode: StatusCodes.Status400BadRequest),
                    BusinessRuleValidationException e => Results.Problem(e.Message, statusCode: StatusCodes.Status400BadRequest),
                    UnauthorizedAccessException e => Results.Problem(e.Message, statusCode: StatusCodes.Status401Unauthorized),
                    NotFoundException e => Results.Problem(e.Message, statusCode: StatusCodes.Status404NotFound),
                    _ => Results.Problem("Internal server error", statusCode: StatusCodes.Status500InternalServerError)
                };

                await result.ExecuteAsync(context);
            });
        });

        return app;
    }
}