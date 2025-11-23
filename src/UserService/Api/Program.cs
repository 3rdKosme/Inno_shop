using Inno_Shop.UserService.Application;
using Inno_Shop.UserService.Infrastructure;
using Inno_Shop.UserService.Api;
using Inno_Shop.UserService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApi(builder.Configuration);

var app = builder.Build();

app.UseApi();
app.ApplyMigration();
app.MapControllers();

app.Run();

