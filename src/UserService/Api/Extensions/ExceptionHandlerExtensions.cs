using Inno_Shop.UserService.Application.Exceptions;
using FluentValidation;
using Inno_Shop.Shared.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Inno_Shop.UserService.Api.Extensions;

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
                    EmailAlreadyExistsException e => Results.Problem(e.Message, statusCode: StatusCodes.Status409Conflict),
                    InvalidCredentialsException e => Results.Problem(e.Message, statusCode: StatusCodes.Status401Unauthorized),
                    NotFoundException e => Results.Problem(e.Message, statusCode: StatusCodes.Status404NotFound),
                    TokenIsExpiredOrRevokedException e => Results.Problem(e.Message, statusCode: StatusCodes.Status406NotAcceptable),
                    _ => Results.Problem("Internal server error", statusCode: StatusCodes.Status500InternalServerError)
                };

                await result.ExecuteAsync(context);
            });
        });

        return app;
    }
}