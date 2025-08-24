using System.Collections.Generic;
using System.Threading.Tasks;
using ShoukatSons.Data.Entities;
using ShoukatSons.Services.Interfaces.Dtos;

namespace ShoukatSons.Services.Interfaces
{
    public interface IReturnExchangeService
    {
        Task<Return> CreateReturnAsync(
            int? relatedSaleId,
            IEnumerable<ReturnLineInput> lines,
            string? reason,
            int approverUserId);

        Task<Exchange> CreateExchangeAsync(
            int originalSaleId,
            IEnumerable<ReturnLineInput> returnLines,
            IEnumerable<SaleLineInput> newSaleLines,
            IEnumerable<PaymentInput> paymentsOrRefunds,
            string? reason,
            int approverUserId);
    }
}