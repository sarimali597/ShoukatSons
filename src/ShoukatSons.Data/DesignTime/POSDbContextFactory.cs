using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShoukatSons.Data.DesignTime
{
    /// <summary>
    /// Design-time factory for POSDbContext.
    /// Ensures EF Core tools (migrations, scaffolding) can create the context
    /// without running the full application.
    /// </summary>
    public class POSDbContextFactory : IDesignTimeDbContextFactory<POSDbContext>
    {
        public POSDbContext CreateDbContext(string[] args)
        {
            // Central, machine-wide database location
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "ShoukatSons",
                "data");

            Directory.CreateDirectory(dir);

            var dbPath = Path.Combine(dir, "pos.db");

            var options = new DbContextOptionsBuilder<POSDbContext>()
                .UseSqlite($"Data Source={dbPath};")
                .Options;

            return new POSDbContext(options);
        }
    }
}