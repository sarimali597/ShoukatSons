// File: src/ShoukatSons.Core/POS/PosDocument.cs
namespace ShoukatSons.Core.POS
{
    public class PosDocument
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public List<PosDocumentLine> Lines { get; set; } = new();

        // ✅ Add these if used by services
        public List<PosPayment> Payments { get; set; } = new();
        public Guid? ExchangeGroupId { get; set; }
    }
}