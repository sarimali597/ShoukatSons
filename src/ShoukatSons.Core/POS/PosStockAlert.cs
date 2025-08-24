namespace ShoukatSons.Core.POS
{
    public class PosStockAlert
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal Threshold { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool IsDismissed => DismissedAtUtc.HasValue;
        public DateTime? DismissedAtUtc { get; set; }
    }
}