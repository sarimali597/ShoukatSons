using System.Threading;
using System.Threading.Tasks;

namespace ShoukatSons.UI.Services.Interfaces
{
    public record ProductLookupDto(string Barcode, string Name, string? Size, string? Color, decimal SalePrice);

    public interface IProductLookupService
    {
        Task<ProductLookupDto?> GetByBarcodeAsync(string barcode, CancellationToken ct = default);
    }
}