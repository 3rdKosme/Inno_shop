using Inno_Shop.ProductService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inno_Shop.ProductService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<Product> Products { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Name).IsRequired().HasMaxLength(128);
            entity.Property(p => p.Description).IsRequired().HasMaxLength(512);
            entity.Property(p => p.Price).IsRequired();
            entity.Property(p => p.IsAvailable).IsRequired();
            entity.Property(p => p.UserId).IsRequired();
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.Property(p => p.IsDeleted).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}