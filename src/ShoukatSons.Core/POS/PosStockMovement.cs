// File: src/ShoukatSons.Core/POS/PosStockMovement.cs
using System;

namespace ShoukatSons.Core.POS
{
    public class PosStockMovement
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        // Quantity change: positive for inbound, negative for outbound
        public decimal QuantityChange { get; set; }

        // Enum reason used across services
        public PosStockMovementReason Reason { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Link to a POS document if applicable (sale, return, exchange)
        public Guid? RelatedDocumentId { get; set; }
    }
}