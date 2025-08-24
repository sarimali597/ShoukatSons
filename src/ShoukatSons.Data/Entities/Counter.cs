namespace ShoukatSons.Data.Entities
{
    public class Counter
    {
        public int Id { get; set; }

        // Used by your services to track different sequences (e.g. "Product", "Invoice")
        public string Name { get; set; } = string.Empty;

        // Last stored counter value (was your service's `Value`)
        public int Value { get; set; }

        // When that counter was last updated (was your service's `UpdatedAt`)
        public DateTime UpdatedAt { get; set; }
    }
}