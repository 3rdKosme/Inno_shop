using Inno_Shop.UserService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inno_Shop.UserService.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}