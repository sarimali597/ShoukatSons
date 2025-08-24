using System.Collections.Generic;
using System.Threading.Tasks;
using ShoukatSons.Data.Entities;
using ShoukatSons.Services.Interfaces.Dtos;

namespace ShoukatSons.Services.Interfaces
{
    public interface ISalesService
    {
        Task<Sale> CreateSaleAsync(
            IEnumerable<SaleLineInput> linesIn,
            decimal? billDiscountPercent,
            decimal? billDiscountAmount,
            IEnumerable<PaymentInput> paymentsIn,
            string? remarks,
            int createdByUserId);
    }
}