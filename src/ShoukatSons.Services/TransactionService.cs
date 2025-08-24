using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Core.POS;
using ShoukatSons.Data;
using ShoukatSons.Data.Entities;
using ShoukatSons.Data.Mappings;

namespace ShoukatSons.Services.Services
{
    public class TransactionService
    {
        private readonly POSDbContext _context;

        public TransactionService(POSDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> ProcessSaleAsync(SaleRequest req, Guid createdByUserId)
        {
            var docId = Guid.NewGuid();
            var document = new Document
            {
                Id              = docId,
                Type            = PosDocumentType.Sale,
                CreatedAtUtc    = DateTime.UtcNow,
                Number          = await GenerateDocumentNumberAsync(PosDocumentType.Sale),
                CustomerName    = null,
                ExchangeGroupId = null
            };

            decimal subtotal = 0m;

            foreach (var lineReq in req.Lines)
            {
                var prod = await _context.PosProducts
                    .SingleOrDefaultAsync(p => p.Sku == lineReq.Sku)
                    ?? throw new InvalidOperationException($"SKU '{lineReq.Sku}' not found.");

                var line = lineReq.ToDocumentLine(docId);
                line.ProductId = prod.Id; // now int
                line.UnitPrice = lineReq.OverrideUnitPrice ?? prod.SalePrice;

                var rawTotal = line.UnitPrice * line.Quantity;
                var lineDiscount = lineReq.DiscountType switch
                {
                    PosDiscountType.FixedAmount => lineReq.DiscountValue,
                    PosDiscountType.Percent     => rawTotal * (lineReq.DiscountValue / 100m),
                    _                           => 0m
                };
                var lineTotal = rawTotal - lineDiscount;

                line.LineDiscount = lineDiscount;
                line.LineTotal    = lineTotal;

                document.Lines.Add(line);
                subtotal += lineTotal;

                var txn = line.ToStockTxn(
                    StockTxnType.Out,
                    createdByUserId,
                    reference: document.Number,
                    note: req.Notes
                );
                await _context.PosStockMovements.AddAsync(txn);

                var inv = await _context.PosInventories
                    .SingleOrDefaultAsync(i => i.ProductId == prod.Id) // both int now
                    ?? throw new InvalidOperationException($"Inventory missing for '{lineReq.Sku}'.");

                inv.ApplyChange(-line.Quantity);
            }

            var billDisc = req.BillDiscountType switch
            {
                PosDiscountType.FixedAmount => req.BillDiscountValue,
                PosDiscountType.Percent     => subtotal * (req.BillDiscountValue / 100m),
                _                           => 0m
            };
            document.Total = subtotal - billDisc;

            foreach (var payReq in req.Payments)
            {
                var payment = payReq.ToPayment(docId, createdByUserId);
                document.Payments.Add(payment);
            }

            await _context.PosDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
            return docId;
        }

        public async Task<Guid> ProcessReturnAsync(ReturnRequest req, Guid createdByUserId)
        {
            var docId = Guid.NewGuid();
            var document = new Document
            {
                Id              = docId,
                Type            = PosDocumentType.Return,
                CreatedAtUtc    = DateTime.UtcNow,
                Number          = await GenerateDocumentNumberAsync(PosDocumentType.Return),
                CustomerName    = null,
                ExchangeGroupId = null
            };

            decimal totalReturn = 0m;

            foreach (var lineReq in req.Lines)
            {
                var prod = await _context.PosProducts
                    .SingleOrDefaultAsync(p => p.Sku == lineReq.Sku)
                    ?? throw new InvalidOperationException($"SKU '{lineReq.Sku}' not found.");

                var line = lineReq.ToDocumentLine(docId);
                line.ProductId = prod.Id; // now int
                line.UnitPrice = prod.SalePrice;
                line.LineTotal = line.UnitPrice * line.Quantity;

                document.Lines.Add(line);
                totalReturn += line.LineTotal;

                var txn = line.ToStockTxn(
                    StockTxnType.Return,
                    createdByUserId,
                    reference: document.Number,
                    note: req.Notes
                );
                await _context.PosStockMovements.AddAsync(txn);

                var inv = await _context.PosInventories
                    .SingleOrDefaultAsync(i => i.ProductId == prod.Id) // both int now
                    ?? throw new InvalidOperationException($"Inventory missing for '{lineReq.Sku}'.");

                inv.ApplyChange(+line.Quantity);
            }

            document.Total = totalReturn;

            foreach (var refundReq in req.Refunds)
            {
                var refund = refundReq.ToPayment(docId, createdByUserId);
                document.Payments.Add(refund);
            }

            await _context.PosDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
            return docId;
        }

        public async Task<(Guid ReturnId, Guid SaleId)> ProcessExchangeAsync(ExchangeRequest req, Guid createdByUserId)
        {
            var exchangeGroupId = Guid.NewGuid();

            var returnReq = new ReturnRequest(req.ReturnLines, new List<PaymentRequest>(), req.Notes);
            var returnId  = await InnerProcessReturn(returnReq, createdByUserId, exchangeGroupId);

            var saleReq = new SaleRequest(
                req.SaleLines,
                req.BillDiscountType,
                req.BillDiscountValue,
                req.Payments,
                req.Notes
            );
            var saleId   = await InnerProcessSale(saleReq, createdByUserId, exchangeGroupId);

            return (returnId, saleId);
        }

        private async Task<Guid> InnerProcessSale(SaleRequest req, Guid userId, Guid exchangeGroupId)
        {
            var docId = await ProcessSaleAsync(req, userId);
            var doc   = await _context.PosDocuments.FindAsync(docId)
                       ?? throw new InvalidOperationException($"Sale document '{docId}' not found.");

            doc.ExchangeGroupId = exchangeGroupId;
            await _context.SaveChangesAsync();
            return docId;
        }

        private async Task<Guid> InnerProcessReturn(ReturnRequest req, Guid userId, Guid exchangeGroupId)
        {
            var docId = await ProcessReturnAsync(req, userId);
            var doc   = await _context.PosDocuments.FindAsync(docId)
                       ?? throw new InvalidOperationException($"Return document '{docId}' not found.");

            doc.ExchangeGroupId = exchangeGroupId;
            await _context.SaveChangesAsync();
            return docId;
        }

        private async Task<string> GenerateDocumentNumberAsync(PosDocumentType type)
        {
            var date   = DateTime.UtcNow.Date;
            var prefix = type.ToString().ToUpperInvariant().Substring(0, 3);

            var count  = await _context.PosDocuments
                .CountAsync(d => d.Type == type && d.CreatedAtUtc.Date == date);

            return $"{prefix}-{date:yyyyMMdd}-{count + 1:D4}";
        }
    }
}