using System;

namespace ShoukatSons.POS.Wpf.Services.Print
{
    /// <summary>
    /// Basic implementation of IPrintService.
    /// </summary>
    public class PrintService : IPrintService
    {
        public void Print(string content)
        {
            // TODO: replace with real printer integration.
            Console.WriteLine($"[PrintService] Printing: {content}");
        }
    }
}