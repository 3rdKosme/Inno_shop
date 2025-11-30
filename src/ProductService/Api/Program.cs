using Inno_Shop.ProductService.Application;
using Inno_Shop.ProductService.Infrastructure;
using Inno_Shop.ProductService.Api;
using Inno_Shop.ProductService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices()
    .AddInfrastructure(builder.Configuration, builder.Environment)
    .AddApi(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

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
