using Inno_Shop.UserService.Application;
using Inno_Shop.UserService.Infrastructure;
using Inno_Shop.UserService.Api;
using Inno_Shop.UserService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration, builder.Environment)
    .AddApi(builder.Configuration);

var app = builder.Build();

app.UseApi();
if (!app.Environment.IsEnvironment("Testing"))
{
    app.ApplyMigration();
}
app.MapControllers();

app.Run();

public partial class Program
{
}