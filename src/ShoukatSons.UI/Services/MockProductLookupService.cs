using System.Threading;
using System.Threading.Tasks;
using ShoukatSons.UI.Services.Interfaces;

namespace ShoukatSons.UI.Services
{
    public class MockProductLookupService : IProductLookupService
    {
        public Task<ProductLookupDto?> GetByBarcodeAsync(string barcode, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return Task.FromResult<ProductLookupDto?>(null);
            return Task.FromResult<ProductLookupDto?>(new ProductLookupDto(
                barcode,
                "Sample Product",
                "L",
                "Black",
                1499m
            ));
        }
    }
}