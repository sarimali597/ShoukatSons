using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading;
using System.Threading.Tasks;

namespace ShoukatSons.Services
{
    /// <summary>
    /// Contract for printing text receipts asynchronously with progress and cancellation support.
    /// </summary>
    public interface IPrintService : IDisposable
    {
        event Action<int>? PagePrinted;
        event Action<Exception>? PrintFailed;

        void Configure(string printerName, string content, string? fontName = null, float? fontSize = null);

        /// <summary>
        /// Asynchronously sends the configured document to the printer.
        /// </summary>
        Task PrintAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Synchronous wrapper so existing ViewModels can simply call Print().
        /// </summary>
        void Print();
    }

    /// <summary>
    /// GDI+–based print service with pagination, progress events, and cancellation.
    /// </summary>
    public sealed class PrintService : IPrintService
    {
        private string[] _lines = Array.Empty<string>();
        private int _currentLineIndex;
        private Font _font = new Font("Consolas", 9, FontStyle.Regular, GraphicsUnit.Point);
        private readonly object _sync = new();

        public event Action<int>? PagePrinted;
        public event Action<Exception>? PrintFailed;

        public void Configure(string printerName, string content, string? fontName = null, float? fontSize = null)
        {
            lock (_sync)
            {
                _lines = (content ?? string.Empty)
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                _currentLineIndex = 0;

                if (!string.IsNullOrWhiteSpace(fontName) && fontSize.HasValue)
                {
                    _font.Dispose();
                    _font = new Font(fontName, fontSize.Value, FontStyle.Regular, GraphicsUnit.Point);
                }
            }
        }

        public async Task PrintAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(() =>
                {
                    using var doc = new PrintDocument();
                    doc.PrintPage += OnPrintPage;
                    try { doc.Print(); }
                    finally { doc.PrintPage -= OnPrintPage; }
                }, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                PrintFailed?.Invoke(ex);
                throw;
            }
        }

        public void Print()
        {
            // Block and delegate to the async implementation
            PrintAsync().GetAwaiter().GetResult();
        }

        private void OnPrintPage(object? sender, PrintPageEventArgs e)
        {
            try
            {
                var g     = e.Graphics ?? throw new InvalidOperationException("Graphics is null");
                float left= e.MarginBounds.Left;
                float top = e.MarginBounds.Top;
                float lh  = _font.GetHeight(g) + 2;
                float ph  = e.MarginBounds.Height;
                int   max = (int)Math.Floor(ph / lh);
                int   count = 0;
                float y = top;

                while (_currentLineIndex < _lines.Length && count < max)
                {
                    g.DrawString(_lines[_currentLineIndex] ?? string.Empty,
                                 _font, Brushes.Black,
                                 new RectangleF(left, y, e.MarginBounds.Width, lh));
                    _currentLineIndex++;
                    count++;
                    y += lh;
                }

                PagePrinted?.Invoke(count);
                e.HasMorePages = _currentLineIndex < _lines.Length;
            }
            catch (Exception ex)
            {
                PrintFailed?.Invoke(ex);
                e.HasMorePages = false;
            }
        }

        public void Dispose()
        {
            _font.Dispose();
        }
    }
}