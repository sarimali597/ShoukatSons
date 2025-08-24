using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoukatSons.Data;
using ShoukatSons.Data.Entities;

namespace ShoukatSons.Data.Seeding
{
    public class DatabaseSeeder
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogger<DatabaseSeeder>         _logger;

        public DatabaseSeeder(
            IDbContextFactory<AppDbContext> contextFactory,
            ILogger<DatabaseSeeder>         logger)
        {
            _contextFactory = contextFactory;
            _logger         = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            // 1) Create a DbContext and apply any pending migrations
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Applying migrations...");
            await context.Database.MigrateAsync(cancellationToken);

            // 2) Start a transaction to keep seeding atomic
            await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // —— 2a) Seed Categories —————————————————————
                var requiredCategoryNames = new[]
                {
                    "Men's Wear",
                    "Women's Wear",
                    "Kids",
                    "Accessories"
                };

                // fetch any already‐present categories
                var existingCategories = await context.Categories
                    .AsNoTracking()
                    .Where(c => requiredCategoryNames.Contains(c.Name!))
                    .ToListAsync(cancellationToken);

                var existingNames = new HashSet<string>(
                    existingCategories.Select(c => c.Name!),
                    StringComparer.OrdinalIgnoreCase);

                var missingNames = requiredCategoryNames
                    .Where(n => !existingNames.Contains(n))
                    .ToList();

                if (missingNames.Any())
                {
                    _logger.LogInformation(
                        "Inserting {Count} missing categories: {Names}",
                        missingNames.Count,
                        string.Join(", ", missingNames));

                    var toInsertCats = missingNames
                        .Select(n => new Category { Name = n })
                        .ToList();

                    await context.Categories.AddRangeAsync(toInsertCats, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    _logger.LogInformation(
                        "All required categories already exist — skipping.");
                }

                // build a name→id map
                var categoryIds = await context.Categories
                    .AsNoTracking()
                    .Where(c => requiredCategoryNames.Contains(c.Name!))
                    .ToDictionaryAsync(c => c.Name!, c => c.Id, cancellationToken);

                // —— 2b) Seed Products ——————————————————————  
                // define your seed list
                var seedProducts = new List<(
                    string Name,
                    string CategoryName,
                    string Barcode,
                    decimal SalePrice,
                    decimal PurchasePrice,
                    int    StockQuantity) >
                {
                    ("Men's T-Shirt (Black)", "Men's Wear",    "8901000000011", 1200m, 700m, 30),
                    ("Women's Kurti (Blue)",  "Women's Wear",  "8901000000012", 2500m, 1500m, 20),
                    ("Kids Polo (Red)",       "Kids",          "8901000000013",  900m, 500m,  25),
                    ("Leather Belt",          "Accessories",   "8901000000014", 1600m, 900m,  15),
                };

                // pull existing barcodes
                var seedBarcodes = seedProducts.Select(p => p.Barcode).ToList();
                var existingBarcodes = await context.Products
                    .AsNoTracking()
                    .Where(p => seedBarcodes.Contains(p.Barcode!))
                    .Select(p => p.Barcode!)
                    .ToListAsync(cancellationToken);

                var toInsertProds = new List<Product>();

                foreach (var p in seedProducts)
                {
                    if (existingBarcodes.Contains(p.Barcode))
                        continue;

                    // ensure category exists & get its Id
                    if (!categoryIds.TryGetValue(p.CategoryName, out var catId))
                    {
                        var cat = await context.Categories
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Name == p.CategoryName, cancellationToken);

                        if (cat == null)
                        {
                            _logger.LogWarning(
                                "Category '{Name}' missing for product {Prod}. Creating it.",
                                p.CategoryName,
                                p.Name);

                            cat = new Category { Name = p.CategoryName };
                            await context.Categories.AddAsync(cat, cancellationToken);
                            await context.SaveChangesAsync(cancellationToken);
                        }

                        catId = cat.Id;
                        categoryIds[p.CategoryName] = catId;
                    }

                    toInsertProds.Add(new Product
                    {
                        Name           = p.Name,
                        CategoryId     = catId,
                        Barcode        = p.Barcode,
                        SalePrice      = p.SalePrice,
                        PurchasePrice  = p.PurchasePrice,
                        StockQuantity  = p.StockQuantity,
                        Size           = string.Empty,
                        Color          = string.Empty,
                        IsActive       = true,
                        CreatedAt      = DateTime.UtcNow
                    });
                }

                if (toInsertProds.Any())
                {
                    _logger.LogInformation(
                        "Inserting {Count} new products.", toInsertProds.Count);

                    await context.Products.AddRangeAsync(toInsertProds, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    _logger.LogInformation(
                        "All seed products already exist — skipping product insert.");
                }

                // 3) Commit the transaction
                await tx.CommitAsync(cancellationToken);
                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred during database seeding. Rolling back.");

                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}