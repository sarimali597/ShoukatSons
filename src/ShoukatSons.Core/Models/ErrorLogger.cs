using System;
using System.IO;
using System.Threading;

namespace ShoukatSons.Core.Helpers
{
    public static class ErrorLogger
    {
        private static readonly string LogDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "ShoukatSons", "Logs");
        private static readonly string LogFile = Path.Combine(LogDir, "error.log");
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public static void Log(Exception ex)
        {
            try
            {
                Directory.CreateDirectory(LogDir);
                var msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}\n";
                _lock.EnterWriteLock();
                File.AppendAllText(LogFile, msg);
            }
            catch { /* ignore */ }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public static void Log(string message)
        {
            try
            {
                Directory.CreateDirectory(LogDir);
                var msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
                _lock.EnterWriteLock();
                File.AppendAllText(LogFile, msg);
            }
            catch { /* ignore */ }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }
    }
}