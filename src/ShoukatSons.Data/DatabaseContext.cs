// File: src/ShoukatSons.Data/DatabaseContext.cs
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Core.Models;
using System;
using System.IO;

namespace ShoukatSons.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Product>   Products    => Set<Product>();
        public DbSet<Category>  Categories  => Set<Category>();
        public DbSet<Vendor>    Vendors     => Set<Vendor>();
        public DbSet<Sale>      Sales       => Set<Sale>();
        public DbSet<SaleItem>  SaleItems   => Set<SaleItem>();
        public DbSet<User>      Users       => Set<User>();

        // Low-stock alerts
        public DbSet<StockAlert> StockAlerts => Set<StockAlert>();

        public string DbPath { get; }

        public DatabaseContext()
        {
            var dataDir = Path.Combine(@"C:\ShoukatSons", "data");
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);
            DbPath = Path.Combine(dataDir, "shoukatsons.db");
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            DbPath = string.Empty;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={DbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Barcode)
                .IsUnique();

            modelBuilder.Entity<SaleItem>()
                .Property(si => si.LineTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.PurchasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.SalePrice)
                .HasPrecision(18, 2);

            // StockAlert → Product (N:1)
            modelBuilder.Entity<StockAlert>()
                .HasOne(a => a.Product)
                .WithMany()
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Helpful indexes for alert queries
            modelBuilder.Entity<StockAlert>()
                .HasIndex(a => new { a.ProductId, a.AlertedAt });

            modelBuilder.Entity<StockAlert>()
                .HasIndex(a => a.DismissedAt);

            // Seed Admin user (default password SSG@2025!)
            var admin = new User
            {
                Id           = 1,
                Username     = "Admin",
                PasswordHash = ShoukatSons.Core.Helpers.PasswordHasher.Hash("SSG@2025!"),
                Role         = UserRole.Admin,
                IsActive     = true
            };
            modelBuilder.Entity<User>().HasData(admin);
        }
    }
}