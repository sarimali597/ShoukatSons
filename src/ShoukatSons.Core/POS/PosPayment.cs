// File: src/ShoukatSons.Core/POS/PosPayment.cs
using System;

namespace ShoukatSons.Core.POS
{
    public class PosPayment
    {
        public Guid Id { get; set; }

        // The related POS document this payment is for
        public Guid PosDocumentId { get; set; }

        // Payment type: Cash, Card, BankTransfer, MobileWallet
        public PosPaymentMethod Method { get; set; }

        // Amount paid in this transaction
        public decimal Amount { get; set; }

        // Currency code (optional, useful for multi‑currency shops)
        public string Currency { get; set; } = "PKR";

        // When the payment was recorded
        public DateTime PaidAtUtc { get; set; }

        // User who recorded the payment
        public Guid? CreatedByUserId { get; set; }
    }
}