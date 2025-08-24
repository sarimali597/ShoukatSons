// File: src/ShoukatSons.Data/POSDbContext.cs
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Data.Entities;

namespace ShoukatSons.Data
{
    public class POSDbContext : DbContext
    {
        public POSDbContext(DbContextOptions<POSDbContext> options)
            : base(options)
        {
        }

        // Use the existing EF entity types here:
        public DbSet<Product>       PosProducts       { get; set; } = null!;
        public DbSet<Inventory>     PosInventories    { get; set; } = null!;
        public DbSet<StockTxn>      PosStockMovements { get; set; } = null!;
        public DbSet<Document>      PosDocuments      { get; set; } = null!;
        public DbSet<DocumentLine>  PosDocumentLines  { get; set; } = null!;
        public DbSet<Payment>       PosPayments       { get; set; } = null!;
        public DbSet<StockAlert>    PosStockAlerts    { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Map Document → Lines
            builder.Entity<Document>()
                .HasMany(d => d.Lines)
                .WithOne(l => l.Document)
                .HasForeignKey(l => l.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Map Line → Product
            builder.Entity<DocumentLine>()
                .HasOne(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Map Payment → Document
            builder.Entity<Payment>()
                .HasOne(p => p.Document)
                .WithMany(d => d.Payments)
                .HasForeignKey(p => p.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Inventory → Product
            builder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // StockTxn → Product
            builder.Entity<StockTxn>()
                .HasOne(t => t.Product)
                .WithMany()
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockAlert → Product
            builder.Entity<StockAlert>()
                .HasOne(a => a.Product)
                .WithMany()
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Defaults for GUID PKs & timestamps
            builder.Entity<Product>()
                .Property(e => e.Id)
                .HasDefaultValueSql("NEWID()");
            builder.Entity<Document>()
                .Property(e => e.Id)
                .HasDefaultValueSql("NEWID()");
            builder.Entity<DocumentLine>()
                .Property(e => e.Id)
                .HasDefaultValueSql("NEWID()");
            builder.Entity<Inventory>()
                .Property(e => e.Id)
                .HasDefaultValueSql("NEWID()");
            builder.Entity<StockTxn>()
                .Property(e => e.Id)
                .HasDefaultValueSql("NEWID()");
            builder.Entity<StockAlert>()
                .Property(e => e.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Entity<Product>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            builder.Entity<Document>()
                .Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");
            builder.Entity<StockTxn>()
                .Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");
            builder.Entity<Inventory>()
                .Property(e => e.LastUpdatedUtc)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}