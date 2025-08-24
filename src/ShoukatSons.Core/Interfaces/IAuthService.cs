using System.Threading.Tasks;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Core.Interfaces
{
    public interface IAuthService
    {
        Task<User?> SignInAsync(string username, string password);
        Task<User> CreateUserAsync(string username, string password, UserRole role);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}