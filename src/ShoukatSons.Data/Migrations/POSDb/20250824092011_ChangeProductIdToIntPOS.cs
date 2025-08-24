using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoukatSons.Data.Migrations.POSDb
{
    /// <inheritdoc />
    public partial class ChangeProductIdToIntPOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PosDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false, defaultValueSql: "NEWID()"),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Number = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExchangeGroupId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PosProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "NEWID()")
                        .Annotation("Sqlite:Autoincrement", true),
                    Sku = table.Column<string>(type: "TEXT", nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    SalePrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    StockQuantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    LowStockThreshold = table.Column<decimal>(type: "TEXT", nullable: false),
                    Size = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Brand = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDiscontinued = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosProducts_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PosPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Method = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    PaidAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosPayments_PosDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "PosDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PosDocumentLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false, defaultValueSql: "NEWID()"),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    LineDiscount = table.Column<decimal>(type: "TEXT", nullable: false),
                    LineTotal = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosDocumentLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosDocumentLines_PosDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "PosDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PosDocumentLines_PosProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "PosProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosInventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false, defaultValueSql: "NEWID()"),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityOnHand = table.Column<decimal>(type: "TEXT", nullable: false),
                    ReorderLevel = table.Column<decimal>(type: "TEXT", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosInventories_PosProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "PosProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PosStockAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "NEWID()")
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    AlertedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DismissedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosStockAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosStockAlerts_PosProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "PosProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PosStockMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false, defaultValueSql: "NEWID()"),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityChange = table.Column<decimal>(type: "TEXT", nullable: false),
                    TxnType = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    Reference = table.Column<string>(type: "TEXT", nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ProductId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosStockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosStockMovements_PosProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "PosProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosStockMovements_PosProducts_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "PosProducts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosDocumentLines_DocumentId",
                table: "PosDocumentLines",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_PosDocumentLines_ProductId",
                table: "PosDocumentLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PosInventories_ProductId",
                table: "PosInventories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PosPayments_DocumentId",
                table: "PosPayments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_PosProducts_CategoryId",
                table: "PosProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PosStockAlerts_ProductId",
                table: "PosStockAlerts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PosStockMovements_ProductId",
                table: "PosStockMovements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PosStockMovements_ProductId1",
                table: "PosStockMovements",
                column: "ProductId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PosDocumentLines");

            migrationBuilder.DropTable(
                name: "PosInventories");

            migrationBuilder.DropTable(
                name: "PosPayments");

            migrationBuilder.DropTable(
                name: "PosStockAlerts");

            migrationBuilder.DropTable(
                name: "PosStockMovements");

            migrationBuilder.DropTable(
                name: "PosDocuments");

            migrationBuilder.DropTable(
                name: "PosProducts");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
