using FluentValidation;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Infrastructure;
using Inno_Shop.UserService.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using Microsoft.OpenApi.Models;
using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));


var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? throw new Exception("JwtSettings not configured");

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);


builder.Services.AddControllers()
    .AddApplicationPart(Assembly.GetExecutingAssembly())
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "UserService API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Example: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            new List<string>()
        }
    });
});

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

        TokenIsExpiredOrRevokedException tokenIsExpiredOrRevokedException => Results.Problem(
            detail: tokenIsExpiredOrRevokedException.Message,
            statusCode: tokenIsExpiredOrRevokedException.statusCode),

        _ => Results.Problem(
            detail: "Internal server error",
            statusCode: 500)
    };
});

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

