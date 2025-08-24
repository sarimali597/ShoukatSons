using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ShoukatSons.Core.Interfaces;

namespace ShoukatSons.Services
{
    public class BackupService : IBackupService
    {
        private static byte[] DeriveKey(string passphrase, byte[] salt)
        {
            using var kdf = new Rfc2898DeriveBytes(passphrase, salt, 100_000, HashAlgorithmName.SHA256);
            return kdf.GetBytes(32);
        }

        public async Task<string> BackupAsync(string dbPath, string outputDir, string passphrase)
        {
            if (!File.Exists(dbPath)) throw new FileNotFoundException("DB not found", dbPath);
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            var salt = RandomNumberGenerator.GetBytes(16);
            var key = DeriveKey(passphrase, salt);
            var iv = RandomNumberGenerator.GetBytes(16);

            var ts = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var outFile = Path.Combine(outputDir, $"backup_{ts}.db.enc");

            using var aes = Aes.Create();
            aes.Key = key; aes.IV = iv; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;

            await using var fsIn = File.OpenRead(dbPath);
            await using var fsOut = File.Create(outFile);

            // header: SALT(16) + IV(16)
            await fsOut.WriteAsync(salt, 0, salt.Length);
            await fsOut.WriteAsync(iv, 0, iv.Length);

            await using var crypto = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await fsIn.CopyToAsync(crypto);
            await crypto.FlushAsync();

            return outFile;
        }

        public async Task<string> RestoreAsync(string backupFile, string outputDbPath, string passphrase)
        {
            await using var fsIn = File.OpenRead(backupFile);
            var salt = new byte[16]; var iv = new byte[16];
            _ = await fsIn.ReadAsync(salt, 0, 16);
            _ = await fsIn.ReadAsync(iv, 0, 16);

            var key = DeriveKey(passphrase, salt);

            using var aes = Aes.Create();
            aes.Key = key; aes.IV = iv; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;

            var outDir = Path.GetDirectoryName(outputDbPath);
            if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            await using var fsOut = File.Create(outputDbPath);
            await using var crypto = new CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read);
            await crypto.CopyToAsync(fsOut);
            return outputDbPath;
        }
    }
}