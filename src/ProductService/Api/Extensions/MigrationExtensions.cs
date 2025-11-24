using Inno_Shop.ProductService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inno_Shop.ProductService.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}