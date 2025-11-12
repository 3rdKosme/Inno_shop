using FluentValidation;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();


builder.Services.AddControllers()
    .AddApplicationPart(Assembly.GetExecutingAssembly())
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext context) =>
{
    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

    return ex switch
    {
        ValidationException validationException => Results.Problem(
            detail: validationException.Message,
            statusCode: 400),

        BusinessRuleValidationException businessRuleValudationException => Results.Problem(
            detail: businessRuleValudationException.Message,
            statusCode: businessRuleValudationException.statusCode
            ),

        EmailAlreadyExistsException emailAlreadyExistsException => Results.Problem(
            detail: emailAlreadyExistsException.Message,
            statusCode: emailAlreadyExistsException.statusCode),

        InvalidCredentialsException invalidCredentialsException => Results.Problem(
            detail: invalidCredentialsException.Message,
            statusCode: invalidCredentialsException.statusCode),

        NotFoundException notFoundException => Results.Problem(
            detail: notFoundException.Message,
            statusCode: notFoundException.statusCode),

        _ => Results.Problem(
            detail: "Internal server error",
            statusCode: 500)
    };
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

