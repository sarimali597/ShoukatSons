using System;
using System.Security.Cryptography;
using System.Text;

namespace ShoukatSons.Core.Helpers
{
    public static class PasswordHasher
    {
        public static string Hash(string password, string? salt = null)
        {
            salt ??= Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            using var sha256 = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(salt + password);
            var hash = sha256.ComputeHash(combined);
            return $"{salt}:{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string stored)
        {
            var parts = stored.Split(':');
            if (parts.Length != 2) return false;
            var recomputed = Hash(password, parts[0]);
            return string.Equals(recomputed, stored, StringComparison.Ordinal);
        }
    }
}