param(
  [string]$SolutionName = "ShoukatSons",
  [string]$Root = (Get-Location).Path
)

$ErrorActionPreference = "Stop"

function Write-File {
  param([string]$Path, [string]$Content)
  $dir = Split-Path $Path
  if (!(Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
  Set-Content -Path $Path -Value $Content -Encoding UTF8
  Write-Host "Wrote $Path"
}

# 1) Structure
$src = Join-Path $Root "src"
$dataDir = Join-Path $Root "data"
New-Item -ItemType Directory -Force -Path $src | Out-Null
New-Item -ItemType Directory -Force -Path $dataDir | Out-Null

Push-Location $src

# 2) Solution & Projects
dotnet new sln -n $SolutionName

dotnet new classlib -n ShoukatSons.Domain
dotnet new classlib -n ShoukatSons.Infrastructure
dotnet new classlib -n ShoukatSons.Application
dotnet new console  -n ShoukatSons.ConsoleHarness

dotnet sln add ShoukatSons.Domain/ShoukatSons.Domain.csproj
dotnet sln add ShoukatSons.Infrastructure/ShoukatSons.Infrastructure.csproj
dotnet sln add ShoukatSons.Application/ShoukatSons.Application.csproj
dotnet sln add ShoukatSons.ConsoleHarness/ShoukatSons.ConsoleHarness.csproj

dotnet add ShoukatSons.Infrastructure reference ShoukatSons.Domain
dotnet add ShoukatSons.Application reference ShoukatSons.Domain
dotnet add ShoukatSons.Application reference ShoukatSons.Infrastructure
dotnet add ShoukatSons.ConsoleHarness reference ShoukatSons.Application
dotnet add ShoukatSons.ConsoleHarness reference ShoukatSons.Infrastructure
dotnet add ShoukatSons.ConsoleHarness reference ShoukatSons.Domain

# 3) Packages
dotnet add ShoukatSons.Infrastructure package Microsoft.EntityFrameworkCore --version 8.0.7
dotnet add ShoukatSons.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.7
dotnet add ShoukatSons.Infrastructure package Microsoft.Extensions.Logging.Console --version 8.0.0
dotnet add ShoukatSons.Application package Microsoft.Extensions.DependencyInjection --version 8.0.0
dotnet add ShoukatSons.ConsoleHarness package Microsoft.Extensions.DependencyInjection --version 8.0.0
dotnet add ShoukatSons.ConsoleHarness package Microsoft.Extensions.Logging.Console --version 8.0.0

# 4) Domain files
$domain = @"
using System;
using System.Collections.Generic;

namespace ShoukatSons.Domain;

public enum DocumentType { Sale = 1, Return = 2, Exchange = 3, StockAdjustment = 4 }
public enum DiscountType { None = 0, Percent = 1, FixedAmount = 2 }
public enum PaymentMethod { Cash = 1, Card = 2, Online = 3 }
public enum StockMovementReason { Sale = 1, Return = 2, Exchange = 3, Adjustment = 4, Purchase = 5 }

public class Product {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Sku { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Size { get; set; }
    public string? Color { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public string? Barcode { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Inventory {
    public Guid ProductId { get; set; }
    public int QuantityOnHand { get; set; }
    public Product Product { get; set; } = null!;
}

public class Document {
    public Guid Id { get; set; } = Guid.NewGuid();
    public DocumentType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public decimal Subtotal { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal Total { get; set; }

    public Guid? ExchangeGroupId { get; set; }

    public List<DocumentLine> Lines { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
}

public class DocumentLine {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } // positive; direction derived from Document.Type
    public decimal UnitPrice { get; set; }

    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal LineTotal { get; set; }

    public string? OverrideReason { get; set; } // bargaining reason
    public Document Document { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class Payment {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public Document Document { get; set; } = null!;
}

public class StockMovement {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public int QuantityChange { get; set; } // negative for sale, positive for return
    public StockMovementReason Reason { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid? RelatedDocumentId { get; set; }
    public Product Product { get; set; } = null!;
}
"@

Write-File -Path (Join-Path $src "ShoukatSons.Domain/Entities.cs") -Content $domain

# 5) Infrastructure files
$infraCtx = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoukatSons.Domain;

namespace ShoukatSons.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Inventory> Inventory => Set<Inventory>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentLine> DocumentLines => Set<DocumentLine>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    public static readonly LoggerFactory ConsoleLoggerFactory =
        new LoggerFactory(new[] { new Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider((_, __) => true, true) });

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", "data", "pos.db");
        dbPath = System.IO.Path.GetFullPath(dbPath);
        optionsBuilder
            .UseLoggerFactory(ConsoleLoggerFactory)
            .EnableSensitiveDataLogging()
            .UseSqlite($""Data Source={dbPath}"");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b => {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Sku).IsUnique();
            b.HasIndex(x => x.Barcode).IsUnique(false);
            b.Property(x => x.CostPrice).HasPrecision(18,2);
            b.Property(x => x.SalePrice).HasPrecision(18,2);
        });

        modelBuilder.Entity<Inventory>(b => {
            b.HasKey(x => x.ProductId);
            b.HasOne(x => x.Product).WithOne().HasForeignKey<Inventory>(x => x.ProductId);
        });

        modelBuilder.Entity<Document>(b => {
            b.HasKey(x => x.Id);
            b.Property(x => x.Subtotal).HasPrecision(18,2);
            b.Property(x => x.DiscountValue).HasPrecision(18,2);
            b.Property(x => x.Total).HasPrecision(18,2);
            b.HasMany(x => x.Lines).WithOne(x => x.Document).HasForeignKey(x => x.DocumentId);
            b.HasMany(x => x.Payments).WithOne(x => x.Document).HasForeignKey(x => x.DocumentId);
        });

        modelBuilder.Entity<DocumentLine>(b => {
            b.HasKey(x => x.Id);
            b.Property(x => x.UnitPrice).HasPrecision(18,2);
            b.Property(x => x.DiscountValue).HasPrecision(18,2);
            b.Property(x => x.LineTotal).HasPrecision(18,2);
            b.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        });

        modelBuilder.Entity<Payment>(b => {
            b.HasKey(x => x.Id);
            b.Property(x => x.Amount).HasPrecision(18,2);
        });

        modelBuilder.Entity<StockMovement>(b => {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        });
    }
}
"@

Write-File -Path (Join-Path $src "ShoukatSons.Infrastructure/AppDbContext.cs") -Content $infraCtx

# 6) Application DTOs and Services
$appDtos = @"
using ShoukatSons.Domain;

namespace ShoukatSons.Application;

public record SaleLineRequest(
    string Sku,
    int Quantity,
    decimal? OverrideUnitPrice,
    string? OverrideReason,
    DiscountType DiscountType,
    decimal DiscountValue
);

public record PaymentRequest(
    PaymentMethod Method,
    decimal Amount,
    string? Reference
);

public record SaleRequest(
    List<SaleLineRequest> Lines,
    DiscountType BillDiscountType,
    decimal BillDiscountValue,
    List<PaymentRequest> Payments,
    string? Notes
);

public record ReturnLineRequest(
    string Sku,
    int Quantity
);

public record ReturnRequest(
    List<ReturnLineRequest> Lines,
    List<PaymentRequest> Refunds,
    string? Notes
);

public record ExchangeRequest(
    List<ReturnLineRequest> ReturnLines,
    List<SaleLineRequest> SaleLines,
    DiscountType BillDiscountType,
    decimal BillDiscountValue,
    List<PaymentRequest> Payments,
    string? Notes
);
"@

Write-File -Path (Join-Path $src "ShoukatSons.Application/DTOs.cs") -Content $appDtos

$appServices = @"
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Domain;
using ShoukatSons.Infrastructure;

namespace ShoukatSons.Application;

public interface IInventoryService {
    Task<int> GetOnHandAsync(string sku);
    Task AdjustAsync(Guid productId, int delta, StockMovementReason reason, Guid? docId, CancellationToken ct);
}

public interface ITransactionService {
    Task<Document> CreateSaleAsync(SaleRequest request, CancellationToken ct);
    Task<Document> CreateReturnAsync(ReturnRequest request, CancellationToken ct);
    Task<(Document ReturnDoc, Document SaleDoc)> CreateExchangeAsync(ExchangeRequest request, CancellationToken ct);
}

public interface IReportingService {
    Task<(decimal TotalSales, int ItemsSold)> DailySalesAsync(DateTime date, CancellationToken ct);
    Task<List<(string Sku, string Name, int OnHand)>> StockReportAsync(CancellationToken ct);
}

public class InventoryService : IInventoryService {
    private readonly AppDbContext _db;
    public InventoryService(AppDbContext db) { _db = db; }

    public async Task<int> GetOnHandAsync(string sku) {
        var prod = await _db.Products.SingleAsync(p => p.Sku == sku);
        var inv = await _db.Inventory.FindAsync(prod.Id);
        return inv?.QuantityOnHand ?? 0;
    }

    public async Task AdjustAsync(Guid productId, int delta, StockMovementReason reason, Guid? docId, CancellationToken ct) {
        var inv = await _db.Inventory.FindAsync(new object?[] { productId }, ct);
        if (inv == null) {
            inv = new Inventory { ProductId = productId, QuantityOnHand = 0 };
            _db.Inventory.Add(inv);
        }
        inv.QuantityOnHand += delta;
        _db.StockMovements.Add(new StockMovement {
            ProductId = productId,
            QuantityChange = delta,
            Reason = reason,
            RelatedDocumentId = docId
        });
        await _db.SaveChangesAsync(ct);
    }
}

public static class Pricing {
    public static decimal ApplyDiscount(decimal baseAmount, DiscountType type, decimal value) {
        return type switch {
            DiscountType.None => baseAmount,
            DiscountType.Percent => Math.Max(0, baseAmount - Math.Round(baseAmount * (value / 100m), 2)),
            DiscountType.FixedAmount => Math.Max(0, baseAmount - value),
            _ => baseAmount
        };
    }
}

public class TransactionService : ITransactionService {
    private readonly AppDbContext _db;
    private readonly IInventoryService _inventory;

    public TransactionService(AppDbContext db, IInventoryService inventory) {
        _db = db; _inventory = inventory;
    }

    public async Task<Document> CreateSaleAsync(SaleRequest request, CancellationToken ct) {
        using var tx = await _db.Database.BeginTransactionAsync(ct);
        var doc = new Document {
            Type = DocumentType.Sale,
            Notes = request.Notes
        };
        foreach (var line in request.Lines) {
            var product = await _db.Products.SingleAsync(p => p.Sku == line.Sku, ct);
            var unitPrice = line.OverrideUnitPrice ?? product.SalePrice;
            var baseLine = unitPrice * line.Quantity;
            var lineTotal = Pricing.ApplyDiscount(baseLine, line.DiscountType, line.DiscountValue);

            doc.Lines.Add(new DocumentLine {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = unitPrice,
                DiscountType = line.DiscountType,
                DiscountValue = line.DiscountValue,
                LineTotal = lineTotal,
                OverrideReason = line.OverrideReason
            });

            // Stock minus
            await _inventory.AdjustAsync(product.Id, -line.Quantity, StockMovementReason.Sale, doc.Id, ct);
        }

        doc.Subtotal = doc.Lines.Sum(l => l.LineTotal);
        doc.Total = Pricing.ApplyDiscount(doc.Subtotal, request.BillDiscountType, request.BillDiscountValue);
        doc.DiscountType = request.BillDiscountType;
        doc.DiscountValue = request.BillDiscountValue;

        var paySum = request.Payments.Sum(p => p.Amount);
        if (Math.Round(paySum,2) != Math.Round(doc.Total,2))
            throw new InvalidOperationException($""Payment total {paySum} must equal bill total {doc.Total}"");

        foreach (var p in request.Payments) {
            doc.Payments.Add(new Payment { Method = p.Method, Amount = p.Amount, Reference = p.Reference });
        }

        _db.Documents.Add(doc);
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return doc;
    }

    public async Task<Document> CreateReturnAsync(ReturnRequest request, CancellationToken ct) {
        using var tx = await _db.Database.BeginTransactionAsync(ct);
        var doc = new Document {
            Type = DocumentType.Return,
            Notes = request.Notes
        };

        foreach (var line in request.Lines) {
            var product = await _db.Products.SingleAsync(p => p.Sku == line.Sku, ct);
            var unitPrice = product.SalePrice;
            var baseLine = unitPrice * line.Quantity;
            var lineTotal = baseLine; // no line discount on return

            doc.Lines.Add(new DocumentLine {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = unitPrice,
                DiscountType = DiscountType.None,
                DiscountValue = 0m,
                LineTotal = lineTotal
            });

            // Stock plus
            await _inventory.AdjustAsync(product.Id, +line.Quantity, StockMovementReason.Return, doc.Id, ct);
        }

        doc.Subtotal = doc.Lines.Sum(l => l.LineTotal);
        doc.Total = doc.Subtotal;

        var refundSum = request.Refunds.Sum(p => p.Amount);
        if (Math.Round(refundSum,2) != Math.Round(doc.Total,2))
            throw new InvalidOperationException($""Refund total {refundSum} must equal return total {doc.Total}"");

        foreach (var p in request.Refunds) {
            doc.Payments.Add(new Payment { Method = p.Method, Amount = p.Amount, Reference = p.Reference });
        }

        _db.Documents.Add(doc);
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return doc;
    }

    public async Task<(Document ReturnDoc, Document SaleDoc)> CreateExchangeAsync(ExchangeRequest request, CancellationToken ct) {
        using var tx = await _db.Database.BeginTransactionAsync(ct);
        var exchangeGroup = Guid.NewGuid();

        // 1) Return part
        var retDoc = new Document { Type = DocumentType.Return, ExchangeGroupId = exchangeGroup, Notes = request.Notes };
        foreach (var r in request.ReturnLines) {
            var product = await _db.Products.SingleAsync(p => p.Sku == r.Sku, ct);
            var unitPrice = product.SalePrice;
            var baseLine = unitPrice * r.Quantity;

            retDoc.Lines.Add(new DocumentLine {
                ProductId = product.Id,
                Quantity = r.Quantity,
                UnitPrice = unitPrice,
                DiscountType = DiscountType.None,
                DiscountValue = 0m,
                LineTotal = baseLine
            });
            await _inventory.AdjustAsync(product.Id, +r.Quantity, StockMovementReason.Exchange, retDoc.Id, ct);
        }
        retDoc.Subtotal = retDoc.Lines.Sum(l => l.LineTotal);
        retDoc.Total = retDoc.Subtotal;

        // 2) Sale part
        var saleDoc = new Document { Type = DocumentType.Sale, ExchangeGroupId = exchangeGroup, Notes = request.Notes };
        foreach (var s in request.SaleLines) {
            var product = await _db.Products.SingleAsync(p => p.Sku == s.Sku, ct);
            var unitPrice = s.OverrideUnitPrice ?? product.SalePrice;
            var baseLine = unitPrice * s.Quantity;
            var lineTotal = Pricing.ApplyDiscount(baseLine, s.DiscountType, s.DiscountValue);

            saleDoc.Lines.Add(new DocumentLine {
                ProductId = product.Id,
                Quantity = s.Quantity,
                UnitPrice = unitPrice,
                DiscountType = s.DiscountType,
                DiscountValue = s.DiscountValue,
                LineTotal = lineTotal,
                OverrideReason = s.OverrideReason
            });
            await _inventory.AdjustAsync(product.Id, -s.Quantity, StockMovementReason.Exchange, saleDoc.Id, ct);
        }
        saleDoc.Subtotal = saleDoc.Lines.Sum(l => l.LineTotal);
        saleDoc.Total = Pricing.ApplyDiscount(saleDoc.Subtotal, request.BillDiscountType, request.BillDiscountValue);
        saleDoc.DiscountType = request.BillDiscountType;
        saleDoc.DiscountValue = request.BillDiscountValue;

        // Payments must cover difference: sale total - return total
        var netDue = Math.Max(0, saleDoc.Total - retDoc.Total);
        var paySum = request.Payments.Sum(p => p.Amount);
        if (Math.Round(paySum,2) != Math.Round(netDue,2))
            throw new InvalidOperationException($""Payment total {paySum} must equal net due {netDue}"");

        foreach (var p in request.Payments) {
            saleDoc.Payments.Add(new Payment { Method = p.Method, Amount = p.Amount, Reference = p.Reference });
        }

        _db.Documents.AddRange(retDoc, saleDoc);
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return (retDoc, saleDoc);
    }
}

public class ReportingService : IReportingService {
    private readonly AppDbContext _db;
    public ReportingService(AppDbContext db) { _db = db; }

    public async Task<(decimal TotalSales, int ItemsSold)> DailySalesAsync(DateTime date, CancellationToken ct) {
        var start = new DateTime(date.Year, date.Month, date.Day, 0,0,0, DateTimeKind.Utc);
        var end = start.AddDays(1);

        var q = _db.Documents
            .Where(d => d.Type == DocumentType.Sale && d.CreatedAt >= start && d.CreatedAt < end);

        var total = await q.SumAsync(d => (decimal?)d.Total, ct) ?? 0m;
        var items = await q.SelectMany(d => d.Lines).SumAsync(l => (int?)l.Quantity, ct) ?? 0;

        return (total, items);
    }

    public async Task<List<(string Sku, string Name, int OnHand)>> StockReportAsync(CancellationToken ct) {
        var data = await _db.Inventory
            .Include(i => i.Product)
            .OrderBy(i => i.Product.Sku)
            .Select(i => new { i.Product.Sku, i.Product.Name, i.QuantityOnHand })
            .ToListAsync(ct);

        return data.Select(x => (x.Sku, x.Name, x.QuantityOnHand)).ToList();
    }
}
"@

Write-File -Path (Join-Path $src "ShoukatSons.Application/Services.cs") -Content $appServices

# 7) Console harness to demo end-to-end
$program = @"
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Domain;
using ShoukatSons.Infrastructure;
using ShoukatSons.Application;

class Program {
    static async Task Main() {
        var services = new ServiceCollection()
            .AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Information))
            .AddDbContext<AppDbContext>()
            .AddScoped<IInventoryService, InventoryService>()
            .AddScoped<ITransactionService, TransactionService>()
            .AddScoped<IReportingService, ReportingService>()
            .BuildServiceProvider();

        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<AppDbContext>();

        Console.WriteLine(""Ensuring database..."");
        await db.Database.EnsureCreatedAsync();

        // Seed products if empty
        if (!db.Products.Any()) {
            db.Products.AddRange(
                new Product { Sku = ""TS-001"", Name = ""T-Shirt Classic"", Size = ""M"", Color = ""Black"", CostPrice = 500, SalePrice = 1000, Barcode = ""890000000001"" },
                new Product { Sku = ""JN-032"", Name = ""Jeans Slim"", Size = ""32"", Color = ""Blue"", CostPrice = 1800, SalePrice = 3000, Barcode = ""890000000032"" }
            );
            await db.SaveChangesAsync();
        }

        var inventory = sp.GetRequiredService<IInventoryService>();
        var trx = sp.GetRequiredService<ITransactionService>();
        var rpt = sp.GetRequiredService<IReportingService>();

        // Add stock (purchase/adjustment simulated)
        var p1 = await db.Products.SingleAsync(p => p.Sku == ""TS-001"");
        var p2 = await db.Products.SingleAsync(p => p.Sku == ""JN-032"");
        await inventory.AdjustAsync(p1.Id, +50, StockMovementReason.Purchase, null, CancellationToken.None);
        await inventory.AdjustAsync(p2.Id, +30, StockMovementReason.Purchase, null, CancellationToken.None);

        Console.WriteLine($""OnHand TS-001: {await inventory.GetOnHandAsync(""TS-001"")}"");
        Console.WriteLine($""OnHand JN-032: {await inventory.GetOnHandAsync(""JN-032"")}"");

        // SALE: 2x T-shirt (one with item discount), 1x Jeans; bill discount 5%; split payments cash+card
        var sale = await trx.CreateSaleAsync(
            new SaleRequest(
                Lines: new() {
                    new SaleLineRequest(""TS-001"", 1, null, null, DiscountType.None, 0),
                    new SaleLineRequest(""TS-001"", 1, 900m, ""Bargain customer"", DiscountType.FixedAmount, 50m),
                    new SaleLineRequest(""JN-032"", 1, null, null, DiscountType.Percent, 10m)
                },
                BillDiscountType: DiscountType.Percent,
                BillDiscountValue: 5m,
                Payments: new() {
                    new PaymentRequest(PaymentMethod.Cash, 1500m, ""Cash-1""),
                    new PaymentRequest(PaymentMethod.Card, 0m, ""Card-1"") // we'll fix to exact total below
                },
                Notes: ""Initial sale with discounts""
            ),
            CancellationToken.None
        );

        // Fix second payment to match exact total
        var totalSale = sale.Total;
        // Re-run sale with correct split (demo simplicity: cancel and redo would be realistic in UI; here we patch quickly)
        db.Documents.Remove(sale);
        await db.SaveChangesAsync();
        // Re-execute with correct split
        var split1 = Math.Round(totalSale * 0.6m, 2);
        var split2 = Math.Round(totalSale - split1, 2);
        sale = await trx.CreateSaleAsync(
            new SaleRequest(
                Lines: new() {
                    new SaleLineRequest(""TS-001"", 1, null, null, DiscountType.None, 0),
                    new SaleLineRequest(""TS-001"", 1, 900m, ""Bargain customer"", DiscountType.FixedAmount, 50m),
                    new SaleLineRequest(""JN-032"", 1, null, null, DiscountType.Percent, 10m)
                },
                BillDiscountType: DiscountType.Percent,
                BillDiscountValue: 5m,
                Payments: new() {
                    new PaymentRequest(PaymentMethod.Cash, split1, ""Cash-1""),
                    new PaymentRequest(PaymentMethod.Card, split2, ""Card-1"")
                },
                Notes: ""Initial sale with discounts (final)""
            ),
            CancellationToken.None
        );

        Console.WriteLine($""SALE created: {sale.Id} Total={sale.Total}"");
        Console.WriteLine($""OnHand TS-001 after sale: {await inventory.GetOnHandAsync(""TS-001"")}"");
        Console.WriteLine($""OnHand JN-032 after sale: {await inventory.GetOnHandAsync(""JN-032"")}"");

        // RETURN: 1x T-shirt
        var ret = await trx.CreateReturnAsync(
            new ReturnRequest(
                Lines: new() { new ReturnLineRequest(""TS-001"", 1) },
                Refunds: new() { new PaymentRequest(PaymentMethod.Cash, 1000m, ""Refund-1"") }, // using base price for demo
                Notes: ""Customer returned one T-shirt""
            ),
            CancellationToken.None
        );

        Console.WriteLine($""RETURN created: {ret.Id} Total={ret.Total}"");
        Console.WriteLine($""OnHand TS-001 after return: {await inventory.GetOnHandAsync(""TS-001"")}"");

        // EXCHANGE: Return 1x Jeans, Buy 1x T-shirt (bargain price 950)
        var (retDoc, saleDoc) = await trx.CreateExchangeAsync(
            new ExchangeRequest(
                ReturnLines: new() { new ReturnLineRequest(""JN-032"", 1) },
                SaleLines: new() { new SaleLineRequest(""TS-001"", 1, 950m, ""Exchange bargain"", DiscountType.None, 0m) },
                BillDiscountType: DiscountType.None,
                BillDiscountValue: 0m,
                Payments: new() { new PaymentRequest(PaymentMethod.Cash, 0m, ""ExchangeCash"") }, // likely 0 if equal value
                Notes: ""Exchange jeans for t-shirt""
            ),
            CancellationToken.None
        );

        Console.WriteLine($""EXCHANGE created: Return={retDoc.Id} Sale={saleDoc.Id} NetDue={Math.Max(0, saleDoc.Total - retDoc.Total)}"");
        Console.WriteLine($""OnHand TS-001 after exchange: {await inventory.GetOnHandAsync(""TS-001"")}"");
        Console.WriteLine($""OnHand JN-032 after exchange: {await inventory.GetOnHandAsync(""JN-032"")}"");

        // REPORTS
        var (total, items) = await rpt.DailySalesAsync(DateTime.UtcNow, CancellationToken.None);
        Console.WriteLine($""Daily Sales: Total={total} ItemsSold={items}"");

        var stock = await rpt.StockReportAsync(CancellationToken.None);
        Console.WriteLine(""Stock Report:"");
        foreach (var s in stock) Console.WriteLine($""  {s.Sku} {s.Name} -> {s.OnHand}"");
    }
}
"@

Write-File -Path (Join-Path $src "ShoukatSons.ConsoleHarness/Program.cs") -Content $program

# 8) Make all classlibs target net8 and nullable enable
$tfm = "<TargetFramework>net8.0</TargetFramework>`n<Nullable>enable</Nullable>"
(Get-Content ShoukatSons.Domain/ShoukatSons.Domain.csproj) -replace "<TargetFramework>.*</TargetFramework>", $tfm | Set-Content ShoukatSons.Domain/ShoukatSons.Domain.csproj
(Get-Content ShoukatSons.Infrastructure/ShoukatSons.Infrastructure.csproj) -replace "<TargetFramework>.*</TargetFramework>", $tfm | Set-Content ShoukatSons.Infrastructure/ShoukatSons.Infrastructure.csproj
(Get-Content ShoukatSons.Application/ShoukatSons.Application.csproj) -replace "<TargetFramework>.*</TargetFramework>", $tfm | Set-Content ShoukatSons.Application/ShoukatSons.Application.csproj
(Get-Content ShoukatSons.ConsoleHarness/ShoukatSons.ConsoleHarness.csproj) -replace "<TargetFramework>.*</TargetFramework>", $tfm | Set-Content ShoukatSons.ConsoleHarness/ShoukatSons.ConsoleHarness.csproj

Pop-Location

Write-Host "`nSetup complete. Next:"
Write-Host "1) cd src\ShoukatSons.ConsoleHarness"
Write-Host "2) dotnet run"