using System;
using System.Linq;
using System.Collections.Generic;
using ShoukatSons.Core.POS;
using ShoukatSons.Data.Entities;

namespace ShoukatSons.Data.Mappings
{
    public static class PosMappings
    {
        /// <summary>
        /// Maps a SaleLineRequest into a DocumentLine. 
        /// Caller must set ProductId, LineDiscount, and LineTotal after lookup/calculation.
        /// </summary>
        public static DocumentLine ToDocumentLine(this SaleLineRequest req, Guid documentId)
        {
            return new DocumentLine
            {
                Id           = Guid.NewGuid(),
                DocumentId   = documentId,
                ProductId    = 0,              // int default until resolved
                Quantity     = req.Quantity,
                UnitPrice    = req.OverrideUnitPrice ?? 0m,
                LineDiscount = 0m,             // to be calculated in service
                LineTotal    = 0m               // to be calculated in service
            };
        }

        /// <summary>
        /// Maps a ReturnLineRequest into a DocumentLine. 
        /// Caller must set ProductId and LineTotal after lookup.
        /// </summary>
        public static DocumentLine ToDocumentLine(this ReturnLineRequest req, Guid documentId)
        {
            return new DocumentLine
            {
                Id           = Guid.NewGuid(),
                DocumentId   = documentId,
                ProductId    = 0,              // int default until resolved
                Quantity     = req.Quantity,
                UnitPrice    = 0m,              // assume last sale price, set in service
                LineDiscount = 0m,
                LineTotal    = 0m               // to be calculated in service
            };
        }

        /// <summary>
        /// Maps a PaymentRequest into a Payment entity.
        /// </summary>
        public static Payment ToPayment(this PaymentRequest req, Guid documentId, Guid createdByUserId)
        {
            return new Payment
            {
                Id              = Guid.NewGuid(),
                DocumentId      = documentId,
                Method          = req.Method,
                Amount          = req.Amount,
                Currency        = "PKR",
                PaidAtUtc       = DateTime.UtcNow,
                CreatedByUserId = createdByUserId
            };
        }

        /// <summary>
        /// Creates a StockTxn for an outbound or return movement.
        /// </summary>
        public static StockTxn ToStockTxn(
            this DocumentLine line,
            StockTxnType txnType,
            Guid createdByUserId,
            string? reference = null,
            string? note      = null
        )
        {
            var qty = txnType == StockTxnType.In || txnType == StockTxnType.Return
                ? line.Quantity
                : -line.Quantity;

            return new StockTxn
            {
                Id              = Guid.NewGuid(),
                ProductId       = line.ProductId, // now int
                QuantityChange  = qty,
                TxnType         = txnType,
                Type            = txnType,
                Reference       = reference,
                Note            = note,
                CreatedAtUtc    = DateTime.UtcNow,
                CreatedAt       = DateTime.UtcNow,
                CreatedByUserId = createdByUserId,
                DocumentId      = line.DocumentId
            };
        }

        /// <summary>
        /// Updates an Inventory record by applying a quantity delta.
        /// </summary>
        public static void ApplyChange(this Inventory inv, decimal delta)
        {
            inv.QuantityOnHand += delta;
            inv.LastUpdatedUtc  = DateTime.UtcNow;
        }
    }
}