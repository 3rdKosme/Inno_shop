using Inno_Shop.ProductService.Application;
using Inno_Shop.ProductService.Infrastructure;
using Inno_Shop.ProductService.Api;
using Inno_Shop.ProductService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices()
    .AddInfrastructure(builder.Configuration)
    .AddApi(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseApi();
app.ApplyMigration();
app.MapControllers();

app.Run();
