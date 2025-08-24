using System.Printing;
using System.Linq;

namespace ShoukatSons.UI.Services.Printing
{
    public static class PrinterResolver
    {
        public static string[] GetInstalledPrinters()
        {
            using var server = new LocalPrintServer();
            return server.GetPrintQueues().Select(q => q.FullName).ToArray();
        }
    }
}