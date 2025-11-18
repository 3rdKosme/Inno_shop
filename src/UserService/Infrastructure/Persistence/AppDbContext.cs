using Microsoft.EntityFrameworkCore;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Domain.Enums;

namespace Inno_Shop.UserService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
    public DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();

            entity.Property(u => u.Id).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.UserRole).IsRequired();
            entity.Property(u => u.IsEmailConfirmed).IsRequired();
            entity.Property(u => u.IsActive).IsRequired();

            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(x => x.Token).IsRequired().HasMaxLength(256);
            entity.Property(x => x.ExpiresAt).IsRequired();
            entity.Property(x => x.IsRevoked).HasDefaultValue(false);
            entity.HasIndex(x => x.Token).IsUnique();

            entity.HasOne<User>().WithMany(u => u.PasswordResetTokens).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmailConfirmationToken>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(x => x.Token).IsRequired().HasMaxLength(256);
            entity.Property(x => x.ExpiresAt).IsRequired();
            entity.Property(x => x.IsRevoked).HasDefaultValue(false);
            entity.HasIndex(x => x.Token).IsUnique();

            entity.HasOne<User>().WithMany(u => u.EmailConfirmationTokens).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Token).IsRequired().HasMaxLength(256);
            entity.Property(x => x.ExpiresAt).IsRequired();
            entity.Property(x => x.IsRevoked).HasDefaultValue(false);
            entity.HasIndex(x => x.Token).IsUnique();

            entity.HasOne<User>().WithMany(u => u.RefreshTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        var admin = new User() { Id = 1, Name = "admin", Email = "admin@innoshop.local", PasswordHash = "100000;7S4vRC7ZJjsaA1CS+18xRg==;yqKI3ru76delDG4BLgt2HIQQkwiaOHqAJObzEr8CS/o=", UserRole = UserRole.Admin, IsActive = true, IsEmailConfirmed = true, CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 11, 11), DateTimeKind.Utc) };


        modelBuilder.Entity<User>().HasData(admin);

        base.OnModelCreating(modelBuilder);
    }
}