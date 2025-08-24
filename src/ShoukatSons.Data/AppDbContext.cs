using Microsoft.EntityFrameworkCore;
using ShoukatSons.Data.Entities;
using System;
using System.IO;

namespace ShoukatSons.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ——————————————————————
        // Data-layer entities only
        // ——————————————————————
        public DbSet<Product>       Products           { get; set; }
        public DbSet<Category>      Categories         { get; set; }
        public DbSet<Inventory>     Inventories        { get; set; }
        public DbSet<Counter>       Counters           { get; set; }
        public DbSet<StockTxn>      StockTxns          { get; set; }
        public DbSet<StockTxnLine>  StockTxnLines      { get; set; }
        public DbSet<Sale>          Sales              { get; set; }
        public DbSet<SaleLine>      SaleLines          { get; set; }
        public DbSet<Payment>       Payments           { get; set; }
        public DbSet<Return>        Returns            { get; set; }
        public DbSet<ReturnLine>    ReturnLines        { get; set; }
        public DbSet<Exchange>      Exchanges          { get; set; }
        public DbSet<User>          Users              { get; set; }
        public DbSet<AuditLog>      AuditLogs          { get; set; }

        // New table to store triggered low-stock alerts
        public DbSet<StockAlert>    StockAlerts        { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "ShoukatSons");
                Directory.CreateDirectory(dir);

                var path = Path.Combine(dir, "pos.db");
                optionsBuilder.UseSqlite($"Data Source={path};");
            }
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Unique index on Product.Barcode
            b.Entity<Product>()
             .HasIndex(x => x.Barcode)
             .IsUnique();

            // Inventory ↔ Product (1:1)
            b.Entity<Inventory>()
             .HasOne(i => i.Product)
             .WithOne()
             .HasForeignKey<Inventory>(i => i.ProductId)
             .OnDelete(DeleteBehavior.Restrict);

            // Unique index on Sale.InvoiceNo
            b.Entity<Sale>()
             .HasIndex(s => s.InvoiceNo)
             .IsUnique();

            // StockTxnLine → Product (N:1)
            b.Entity<StockTxnLine>()
             .HasOne(l => l.Product)
             .WithMany()
             .HasForeignKey(l => l.ProductId)
             .OnDelete(DeleteBehavior.Restrict);

            // SaleLine → Product (N:1)
            b.Entity<SaleLine>()
             .HasOne(l => l.Product)
             .WithMany()
             .HasForeignKey(l => l.ProductId)
             .OnDelete(DeleteBehavior.Restrict);

            // ReturnLine → Product (N:1)
            b.Entity<ReturnLine>()
             .HasOne(l => l.Product)
             .WithMany()
             .HasForeignKey(l => l.ProductId)
             .OnDelete(DeleteBehavior.Restrict);

            // Configure StockAlert → Product (N:1)
            b.Entity<StockAlert>()
             .HasOne(a => a.Product)
             .WithMany()
             .HasForeignKey(a => a.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}