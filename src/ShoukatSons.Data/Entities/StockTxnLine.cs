using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Data.Entities
{
    public class StockTxnLine
    {
        [Key]
        public int StockTxnLineId { get; set; }

        [Required]
        public int StockTxnId { get; set; }

        public StockTxn StockTxn { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }   // int to match Product.Id

        public Product Product { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Qty { get; set; }

        [Column(TypeName = "REAL")]
        public decimal? UnitCost { get; set; }

        [MaxLength(300)]
        public string? Note { get; set; }
    }
}