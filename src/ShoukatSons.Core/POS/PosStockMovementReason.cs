namespace ShoukatSons.Core.POS
{
    /// <summary>
    /// Reasons for inventory changes in the POS system.
    /// </summary>
    public enum PosStockMovementReason
    {
        Sale,             // Stock decreased by a sale
        Return,           // Stock increased by a customer return
        Exchange,         // Stock changed due to an exchange
        Restock,          // Stock increased by a new purchase or delivery
        Adjustment        // Manual correction or stocktake adjustment
    }
}