using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CommonService.Configs;
using CommonService.Constants;
using CommonService.Extensions;

namespace OrderService.Models;

public class OrderDbContext : DbContext
{
    private readonly ConnectionStrings _connectionStrings;

    public OrderDbContext(
        DbContextOptions<OrderDbContext> options,
        IOptions<ConnectionStrings> connectionStrings) : base(options)
    {
        _connectionStrings = connectionStrings.Value;
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderOutbox> OrderOutbox { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            //.UseSqlServer(_connectionStrings.OrderDbConnection)
            .UseSqlServer(ConnectionsStr.OrderDbConnection)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);
    
        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId);


        modelBuilder.Entity<Product>().HasData(MoqDataExtension.Products());
        
        

        base.OnModelCreating(modelBuilder);
    }
}