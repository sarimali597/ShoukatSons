using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Core.Helpers;
using ShoukatSons.Core.Interfaces;
using ShoukatSons.Core.Models;
using ShoukatSons.Data;

namespace ShoukatSons.Services
{
    public class AuthService : IAuthService
    {
        private readonly DatabaseContext _db;
        public AuthService(DatabaseContext db) { _db = db; }

        public async Task<User?> SignInAsync(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            if (user is null) return null;
            return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
        }

        public async Task<User> CreateUserAsync(string username, string password, UserRole role)
        {
            var user = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                Role = role,
                IsActive = true
            };
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null) return false;
            if (!PasswordHasher.Verify(currentPassword, user.PasswordHash)) return false;
            user.PasswordHash = PasswordHasher.Hash(newPassword);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}