using System.Collections.Generic;

namespace ShoukatSons.Core.POS
{
    public record SaleLineRequest(
        string Sku,
        int Quantity,
        decimal? OverrideUnitPrice,
        string? OverrideReason,
        PosDiscountType DiscountType,
        decimal DiscountValue
    );

    public record PaymentRequest(
        PosPaymentMethod Method,
        decimal Amount,
        string? Reference
    );

    public record SaleRequest(
        List<SaleLineRequest> Lines,
        PosDiscountType BillDiscountType,
        decimal BillDiscountValue,
        List<PaymentRequest> Payments,
        string? Notes
    );

    public record ReturnLineRequest(
        string Sku,
        int Quantity
    );

    public record ReturnRequest(
        List<ReturnLineRequest> Lines,
        List<PaymentRequest> Refunds,
        string? Notes
    );

    public record ExchangeRequest(
        List<ReturnLineRequest> ReturnLines,
        List<SaleLineRequest> SaleLines,
        PosDiscountType BillDiscountType,
        decimal BillDiscountValue,
        List<PaymentRequest> Payments,
        string? Notes
    );
}