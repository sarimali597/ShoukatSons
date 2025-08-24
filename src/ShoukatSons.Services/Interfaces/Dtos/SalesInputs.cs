using ShoukatSons.Data.Entities; // For PaymentMethod enum
using ShoukatSons.Core.POS;

namespace ShoukatSons.Services.Interfaces.Dtos
{
    public record SaleLineInput(
        int ProductId,
        int Qty,
        decimal UnitListPrice,
        decimal UnitSoldPrice,
        string? OverrideReason,
        int? ApprovedByUserId
    );

    public record PaymentInput(
        PosPaymentMethod Method,
        decimal Amount,
        string? Reference
    );

    public record ReturnLineInput(
        int ProductId,
        int Qty,
        decimal UnitRefundPrice
    );
}