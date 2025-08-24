using System.Threading.Tasks;

namespace ShoukatSons.Core.Interfaces
{
    public interface IBackupService
    {
        Task<string> BackupAsync(string dbPath, string outputDir, string passphrase);
        Task<string> RestoreAsync(string backupFile, string outputDbPath, string passphrase);
    }
}