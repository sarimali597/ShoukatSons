// File: src/ShoukatSons.Services/Interfaces/Dtos/SalesDto.cs
using System;
using System.Collections.Generic;
using ShoukatSons.Core.POS;
using ShoukatSons.Data.Entities;

namespace ShoukatSons.Services.Interfaces.Dtos
{
    public record SaleLineDto(
        int SaleLineId,
        int ProductId,
        string ProductName,
        int Qty,
        decimal UnitListPrice,
        decimal UnitSoldPrice,
        decimal LineDiscount,
        string? PriceOverrideReason,
        int? ApprovedByUserId
    );

    public record SalesDto(
        int SaleId,
        string InvoiceNo,
        DateTime SaleDate,
        decimal Subtotal,
        decimal ItemDiscountTotal,
        decimal BillDiscountAmount,
        decimal? BillDiscountPercent,
        decimal GrandTotal,
        decimal PaidTotal,
        string? Remarks,
        int? CreatedByUserId,
        int? ApprovedByUserId,
        int? ExchangeId,
        IEnumerable<SaleLineDto> Lines,
        IEnumerable<PosPaymentDto> Payments // Uses shared DTO
    );
}