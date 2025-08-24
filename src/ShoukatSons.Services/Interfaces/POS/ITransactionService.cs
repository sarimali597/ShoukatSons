using System.Threading;
using System.Threading.Tasks;
using ShoukatSons.Core.POS;

namespace ShoukatSons.Services.Interfaces.POS
{
    public interface ITransactionService
    {
        Task<PosDocument> CreateSaleAsync(SaleRequest request, CancellationToken ct);
        Task<PosDocument> CreateReturnAsync(ReturnRequest request, CancellationToken ct);
        Task<PosDocument> CreateExchangeAsync(ExchangeRequest request, CancellationToken ct);
    }
}