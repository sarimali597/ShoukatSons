namespace ShoukatSons.Core.POS
{
    /// <summary>
    /// Defines how a discount is applied on a sale or line item.
    /// </summary>
    public enum PosDiscountType
    {
        None,        // No discount
        Percent,     // Percentage-based discount (e.g., 10%)
        FixedAmount  // Flat amount discount (e.g., 50 currency units)
    }
}