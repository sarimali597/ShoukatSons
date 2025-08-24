using System;
using System.Printing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Services.Printing
{
    public static class LabelPrintService
    {
        // Typical thermal DPI; adjust if your printer is 300 DPI.
        private const int DefaultDpi = 203;

        public static void PrintWindows(BarcodeLabel label)
        {
            var queue = GetQueue(label.PrinterName);
            var writer = PrintQueue.CreateXpsDocumentWriter(queue);
            var ticket = queue.UserPrintTicket ?? new PrintTicket();

            // Size in pixels at target DPI
            int pxWidth = MmToPx(label.WidthMm, DefaultDpi);
            int pxHeight = MmToPx(label.HeightMm, DefaultDpi);

            for (int i = 0; i < Math.Max(1, label.Quantity); i++)
            {
                var visual = BuildLabelVisual(label, pxWidth, pxHeight);
                writer.Write(visual, ticket);
            }
        }

        private static PrintQueue GetQueue(string? printerName)
        {
            var server = new LocalPrintServer();
            try
            {
                if (!string.IsNullOrWhiteSpace(printerName))
                    return new PrintQueue(server, printerName);
            }
            catch
            {
                // fall back to default if named queue is invalid
            }
            return LocalPrintServer.GetDefaultPrintQueue();
        }

        private static int MmToPx(double mm, int dpi) => (int)Math.Round(mm / 25.4 * dpi);

        private static DrawingVisual BuildLabelVisual(BarcodeLabel label, int pxWidth, int pxHeight)
        {
            const int margin = 4;
            var dv = new DrawingVisual();
            var dpi = VisualTreeHelper.GetDpi(dv).PixelsPerDip;

            using var dc = dv.RenderOpen();

            // Background
            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, pxWidth, pxHeight));

            // Typefaces
            var tfTitle = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
            var tfText  = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal);
            var tfMono  = new Typeface(new FontFamily("Consolas"),  FontStyles.Normal, FontWeights.Normal,   FontStretches.Normal);

            double y = margin;

            // Shop name (top-left)
            if (!string.IsNullOrWhiteSpace(label.ShopName))
            {
                var ft = CreateText(label.ShopName, tfTitle, 13, dpi, pxWidth - 2 * margin);
                dc.DrawText(ft, new Point(margin, y));
                y += ft.Height;
            }

            // Product name
            if (!string.IsNullOrWhiteSpace(label.Name))
            {
                var ft = CreateText(label.Name, tfText, 12, dpi, pxWidth - 2 * margin);
                dc.DrawText(ft, new Point(margin, y));
                y += ft.Height;
            }

            // Size (if any)
            if (!string.IsNullOrWhiteSpace(label.Size))
            {
                var ft = CreateText($"Size: {label.Size}", tfText, 12, dpi, pxWidth - 2 * margin);
                dc.DrawText(ft, new Point(margin, y));
                y += ft.Height;
            }

            // Price (right-aligned on current line)
            if (!string.IsNullOrWhiteSpace(label.Price))
            {
                var ft = CreateText(label.Price, tfText, 13, dpi, pxWidth - 2 * margin);
                dc.DrawText(ft, new Point(pxWidth - ft.Width - margin, y));
                y += ft.Height;
            }

            // Reserve space for human-readable code at bottom
            var hrFontSize = 11.0;
            var hrSample = CreateText("0123456789012", tfMono, hrFontSize, dpi, pxWidth - 2 * margin);
            int hrHeight = (int)Math.Ceiling(hrSample.Height + 2);

            // Barcode area: remaining height minus bottom human-readable margin
            int availableForBarcode = Math.Max(24, (int)Math.Floor(pxHeight - y - hrHeight - margin));
            var barcodeBmp = BarcodeGenerator.Generate(label, pxWidth - 2 * margin, availableForBarcode);

            // Draw barcode centered horizontally
            var barcodeX = (pxWidth - barcodeBmp.PixelWidth) / 2.0;
            dc.DrawImage(barcodeBmp, new Rect(barcodeX, y, barcodeBmp.PixelWidth, barcodeBmp.PixelHeight));

            // Human-readable (bottom centered)
            if (!string.IsNullOrWhiteSpace(label.Barcode))
            {
                var ft = CreateText(label.Barcode, tfMono, hrFontSize, dpi, pxWidth - 2 * margin);
                var hrY = pxHeight - ft.Height - 2;
                dc.DrawText(ft, new Point((pxWidth - ft.Width) / 2.0, hrY));
            }

            return dv;
        }

        private static FormattedText CreateText(string text, Typeface typeFace, double fontSize, double pixelsPerDip, double maxWidth)
        {
            // Create text and shrink font size to fit the max width if necessary.
            var size = fontSize;
            FormattedText ft;

            while (true)
            {
                ft = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeFace,
                    size,
                    Brushes.Black,
                    pixelsPerDip);

                if (ft.Width <= maxWidth || size <= 9) break;
                size -= 0.5;
            }

            // If still wider, hard truncate with ellipsis
            if (ft.Width > maxWidth)
            {
                string ellipsis = "…";
                string s = text;
                while (s.Length > 0)
                {
                    s = s.Substring(0, s.Length - 1);
                    var test = new FormattedText(
                        s + ellipsis,
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        typeFace,
                        size,
                        Brushes.Black,
                        pixelsPerDip);
                    if (test.Width <= maxWidth) { ft = test; break; }
                }
            }

            return ft;
        }
    }
}