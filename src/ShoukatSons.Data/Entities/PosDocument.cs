// File: src/ShoukatSons.Data/Entities/Document.cs
using System;
using System.Collections.Generic;
using ShoukatSons.Core.POS;

namespace ShoukatSons.Data.Entities
{
    public class Document
    {
        public Guid Id { get; set; }
        public PosDocumentType Type { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public string? Number { get; set; }
        public string? CustomerName { get; set; }

        // Required by ReportingService
        public decimal Total { get; set; }

        // Exchange کے گروپ کے لیے
        public Guid? ExchangeGroupId { get; set; }

        public ICollection<DocumentLine> Lines { get; set; } = new List<DocumentLine>();

        // یہاں Payments کلکشن شامل کریں
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}