using Nstech.Application.Abstractions.Data;
using Nstech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Nstech.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(b =>
        {
            b.ToTable("orders");
            b.HasKey(x => x.Id);
            b.Property(x => x.CustomerId).IsRequired();
            b.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            b.Property(x => x.Total).HasPrecision(18, 2).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => x.CustomerId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<OrderItem>(b =>
        {
            b.ToTable("order_items");
            b.HasKey(x => x.Id);
            b.Property(x => x.OrderId).IsRequired();
            b.Property(x => x.ProductId).HasMaxLength(100).IsRequired();
            b.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Quantity).IsRequired();
            b.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
            b.Property(x => x.LineTotal).HasPrecision(18, 2).IsRequired();
            b.HasIndex(x => x.OrderId);
            b.HasIndex(x => x.ProductId);
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("products");
            b.HasKey(x => x.Id);
            b.Property(x => x.ProductId).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.ProductId).IsUnique();
            b.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
            b.Property(x => x.AvailableQuantity).IsRequired();
        });
    }
}
