using System;

namespace ShoukatSons.Services.Models
{
    public class StockAlertDto
    {
        public int      Id        { get; set; }       // ← int
        public int      ProductId { get; set; }       // ← int
        public string   Message   { get; set; } = string.Empty;
        public int      Quantity  { get; set; }
        public DateTime AlertedAt { get; set; }
    }
}