using ShoukatSons.Data.Entities;
using System.Threading.Tasks;

namespace ShoukatSons.Services.Interfaces
{
    public interface ISecurityService
    {
        Task<User?> ValidatePinAsync(string username, string pin);
        bool CanOverridePrice(User user, decimal percentOverrideRequested, decimal cashierLimitPercent);
        Task<int> EnsureSystemUserAsync(); // fallback user for automated tasks
    }
}