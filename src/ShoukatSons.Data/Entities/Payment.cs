// File: src/ShoukatSons.Data/Entities/Payment.cs
using System;
using ShoukatSons.Core.POS;   // PosPaymentMethod

namespace ShoukatSons.Data.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        // Foreign key back to Document
        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = null!;

        public PosPaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PKR";
        public DateTime PaidAtUtc { get; set; }
        public Guid? CreatedByUserId { get; set; }
    }
}