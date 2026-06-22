using Microsoft.EntityFrameworkCore;
using SalesforceTest.Domain.Entities;
using SalesforceTest.Domain.Interfaces;

namespace SalesforceTest.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<SalesforceConnection> SalesforceConnections => Set<SalesforceConnection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<SalesforceConnection>(entity =>
        {
            entity.HasKey(sc => sc.Id);
            entity.Property(sc => sc.InstanceUrl).HasMaxLength(255).IsRequired();
            entity.Property(sc => sc.AccessToken).IsRequired();
            entity.Property(sc => sc.RefreshToken).IsRequired();
            entity.Property(sc => sc.SalesforceUserId).HasMaxLength(100).IsRequired();
            entity.Property(sc => sc.OrganizationId).HasMaxLength(100).IsRequired();
            entity.Property(sc => sc.DisplayName).HasMaxLength(200);
            entity.Property(sc => sc.Email).HasMaxLength(150);
            entity.HasOne(sc => sc.User)
                  .WithMany()
                  .HasForeignKey(sc => sc.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(sc => sc.UserId).IsUnique();
        });
    }
}
