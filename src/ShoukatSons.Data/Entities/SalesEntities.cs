using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Data.Entities
{
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        [Required, MaxLength(40)]
        public string InvoiceNo { get; set; } = "";

        public DateTime SaleDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "REAL")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "REAL")]
        public decimal ItemDiscountTotal { get; set; }

        [Column(TypeName = "REAL")]
        public decimal BillDiscountAmount { get; set; }

        [Column(TypeName = "REAL")]
        public decimal? BillDiscountPercent { get; set; }

        [Column(TypeName = "REAL")]
        public decimal GrandTotal { get; set; }

        [Column(TypeName = "REAL")]
        public decimal PaidTotal { get; set; }

        [MaxLength(400)]
        public string? Remarks { get; set; }

        public int? CreatedByUserId { get; set; }
        public int? ApprovedByUserId { get; set; }
        public int? ExchangeId { get; set; }

        public ICollection<SaleLine> Lines { get; set; } = new List<SaleLine>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public class SaleLine
    {
        [Key]
        public int SaleLineId { get; set; }

        [Required]
        public int SaleId { get; set; }
        public Sale Sale { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Qty { get; set; }

        [Column(TypeName = "REAL")]
        public decimal UnitListPrice { get; set; }

        [Column(TypeName = "REAL")]
        public decimal UnitSoldPrice { get; set; }

        [Column(TypeName = "REAL")]
        public decimal LineDiscount { get; set; }

        [MaxLength(300)]
        public string? PriceOverrideReason { get; set; }

        public int? ApprovedByUserId { get; set; }
    }

    public class Return
    {
        [Key]
        public int ReturnId { get; set; }

        public int? RelatedSaleId { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "REAL")]
        public decimal TotalRefund { get; set; }

        [MaxLength(400)]
        public string? Reason { get; set; }

        public int? CreatedByUserId { get; set; }
        public int? ApprovedByUserId { get; set; }
        public int? ExchangeId { get; set; }

        public ICollection<ReturnLine> Lines { get; set; } = new List<ReturnLine>();
    }

    public class ReturnLine
    {
        [Key]
        public int ReturnLineId { get; set; }

        [Required]
        public int ReturnId { get; set; }
        public Return Return { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Qty { get; set; }

        [Column(TypeName = "REAL")]
        public decimal UnitRefundPrice { get; set; }
    }

    public class Exchange
    {
        [Key]
        public int ExchangeId { get; set; }

        public int OriginalSaleId { get; set; }
        public int ReturnId { get; set; }
        public int NewSaleId { get; set; }

        [Column(TypeName = "REAL")]
        public decimal Difference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedByUserId { get; set; }
        public int? ApprovedByUserId { get; set; }
    }
}