// File: src/ShoukatSons.Services/Interfaces/Dtos/PosPaymentDto.cs
using System;
using ShoukatSons.Core.POS; // For PosPaymentMethod enum

namespace ShoukatSons.Services.Interfaces.Dtos
{
    public class PosPaymentDto
    {
        public Guid Id { get; set; }

        // The related POS document this payment is for
        public Guid PosDocumentId { get; set; }

        // Payment method (Cash, Card, BankTransfer, MobileWallet)
        public PosPaymentMethod Method { get; set; }

        // Amount paid in this transaction
        public decimal Amount { get; set; }

        // Currency code (default: PKR)
        public string Currency { get; set; } = "PKR";

        // When the payment was recorded
        public DateTime PaidAtUtc { get; set; }

        // Optional: User who recorded the payment
        public Guid? CreatedByUserId { get; set; }
    }
}