using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CommonService.Configs;
using CommonService.Constants;
using CommonService.Extensions;
using Microsoft.Extensions.Logging;

namespace StockService.Data
{
    public class StockDbContext : DbContext
    {
        private readonly ConnectionStrings _connectionStrings;
        public StockDbContext(
            DbContextOptions<StockDbContext> options,
            IOptions<ConnectionStrings> connectionStrings) : base(options)
        {
            _connectionStrings = connectionStrings.Value;
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                //.UseSqlServer(_connectionStrings.InventoryDbConnection)
                .UseSqlServer(ConnectionsStr.InventoryDbConnection)
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Product>().HasData(MoqDataExtension.Products());

            base.OnModelCreating(modelBuilder);
        }
    }
}
