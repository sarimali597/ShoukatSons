using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using ShoukatSons.Core.Helpers;

namespace ShoukatSons.Services
{
    public static class BackupManager
    {
        private static string GetBackupDir()
        {
            var fromConfig = ConfigurationManager.AppSettings["BackupDirectory"];
            if (!string.IsNullOrWhiteSpace(fromConfig)) return fromConfig;

            // Fallback: ProgramData\ShoukatSons\Backups
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                "ShoukatSons", "Backups");
        }

        public static void CreateBackup(params string[] filePaths)
        {
            try
            {
                var dir = GetBackupDir();
                Directory.CreateDirectory(dir);
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var zipPath = Path.Combine(dir, $"backup_{timestamp}.zip");

                using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);
                foreach (var file in filePaths)
                {
                    if (File.Exists(file))
                    {
                        zip.CreateEntryFromFile(file, Path.GetFileName(file));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }
    }
}