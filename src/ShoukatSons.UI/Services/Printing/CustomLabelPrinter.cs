// File: src/ShoukatSons.UI/Services/Printing/CustomLabelPrinter.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using ZXing;
using ZXing.Common;
using ShoukatSons.Core.Models;

namespace ShoukatSons.UI.Services.Printing
{
    public sealed class LabelItem
    {
        public string Barcode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string Size { get; set; } = "";
        public string Color { get; set; } = "";
        public decimal Price { get; set; }
        public string ShopName { get; set; } = "ShoukatSons";
        public string SecretCode { get; set; } = "";
    }

    internal sealed class PrintState
    {
        public List<LabelItem> Items = new();
        public int Index = 0;
        public int PerRow = 3;
        public LabelSettings Settings = new();
    }

    public static class CustomLabelPrinter
    {
        public static void PrintLabels(List<LabelItem> items, LabelSettings settings, int perRow)
        {
            if (items == null || items.Count == 0) throw new ArgumentException("No items to print.");
            if (perRow < 1) perRow = 1;

            var state = new PrintState
            {
                Items = items,
                Settings = settings,
                PerRow = perRow
            };

            using var doc = new PrintDocument();

            // Resolve printer (exact match -> normalized match -> default)
            var resolved = ResolvePrinter(settings.PrinterName);
            if (!string.IsNullOrWhiteSpace(resolved))
                doc.PrinterSettings.PrinterName = resolved;

            // Fallback to default if still invalid
            if (!doc.PrinterSettings.IsValid)
            {
                var def = new PrinterSettings().PrinterName;
                if (!string.IsNullOrWhiteSpace(def))
                    doc.PrinterSettings.PrinterName = def;
            }

            if (!doc.PrinterSettings.IsValid)
                throw new InvalidOperationException("No valid printer is available. Please install or select a printer.");

            // Margins and origin
            doc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            doc.OriginAtMargins = false;

            // Apply paper size (try native supported size; else custom)
            ApplyPaperSize(doc, settings.PaperWidthMm, settings.PaperHeightMm);

            // Prefer higher resolution if available (helps small text)
            var hiRes = doc.PrinterSettings.PrinterResolutions
                .Cast<PrinterResolution>()
                .OrderByDescending(r => r.X)
                .FirstOrDefault();
            if (hiRes != null && hiRes.Kind != PrinterResolutionKind.Custom)
                doc.DefaultPageSettings.PrinterResolution = hiRes;

            doc.PrintPage += (_, e) => OnPrintPage(e, state);

            // Safe print: if custom paper is rejected by driver, retry with default
            try
            {
                doc.Print();
            }
            catch (Exception ex1)
            {
                try
                {
                    var fallbackSize = doc.PrinterSettings.DefaultPageSettings?.PaperSize
                                       ?? doc.PrinterSettings.PaperSizes.Cast<PaperSize>().FirstOrDefault();
                    if (fallbackSize != null)
                        doc.DefaultPageSettings.PaperSize = fallbackSize;

                    doc.Print();
                }
                catch (Exception ex2)
                {
                    throw new InvalidOperationException(
                        $"Printing failed. First error: {ex1.Message}. Retry error: {ex2.Message}");
                }
            }
        }

        public static Bitmap? RenderPreviewBitmap(LabelItem item, LabelSettings settings)
        {
            using var writerBmp = CreateBarcodeBitmap(
                item.Barcode,
                Math.Max(80, (int)(settings.BarcodeWidth * 0.9f)), // slightly smaller for cleaner look
                settings.BarcodeHeight);
            if (writerBmp == null) return null;

            // Preview canvas sized around barcode + text stack
            int w = Math.Max(settings.BarcodeWidth + 80, 360);
            int h = Math.Max(settings.BarcodeHeight + 140, 260);

            var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int pad = 20;
            float innerX = pad;
            float innerW = w - pad * 2;
            float y = pad;

            using var fontTitle = new Font("Segoe UI", 9, FontStyle.Bold);   // shop name smaller
            using var font = new Font("Segoe UI", 8, FontStyle.Regular);

            using var sfCenter = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };
            using var sfLeft = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            // Shop name (centered)
            var lineRect = new RectangleF(innerX, y, innerW, fontTitle.GetHeight(g));
            g.DrawString(item.ShopName, fontTitle, Brushes.Black, lineRect, sfCenter);
            y += lineRect.Height + 2;

            // Product details (left for readability)
            var details = $"{item.ProductName}  {item.Size} {item.Color}".Trim();
            lineRect = new RectangleF(innerX, y, innerW, font.GetHeight(g));
            g.DrawString(details, font, Brushes.Black, lineRect, sfLeft);
            y += lineRect.Height + 2;

            // Barcode centered, draw unscaled for crispness
            int barcodeX = (int)(innerX + (innerW - writerBmp.Width) / 2f);
            var oldInterp = g.InterpolationMode;
            var oldSmooth = g.SmoothingMode;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.SmoothingMode = SmoothingMode.None;
            g.DrawImageUnscaled(writerBmp, barcodeX, (int)y);
            g.InterpolationMode = oldInterp;
            g.SmoothingMode = oldSmooth;

            y += writerBmp.Height + 4;

            // Human-readable barcode number (centered)
            lineRect = new RectangleF(innerX, y, innerW, font.GetHeight(g));
            g.DrawString(item.Barcode, font, Brushes.Black, lineRect, sfCenter);
            y += lineRect.Height + 2;

            // Price (left)
            lineRect = new RectangleF(innerX, y, innerW, font.GetHeight(g));
            g.DrawString($"Price: {item.Price:0}", font, Brushes.Black, lineRect, sfLeft);
            y += lineRect.Height + 2;

            // Secret code (left, optional)
            if (!string.IsNullOrWhiteSpace(item.SecretCode))
            {
                lineRect = new RectangleF(innerX, y, innerW, font.GetHeight(g));
                g.DrawString($"Code: {item.SecretCode}", font, Brushes.Black, lineRect, sfLeft);
            }

            return (Bitmap)bmp.Clone();
        }

        private static void OnPrintPage(PrintPageEventArgs e, PrintState state)
        {
            var g = e.Graphics ?? throw new InvalidOperationException("Graphics unavailable");

            // Crisp barcodes, readable text
            g.PageUnit = GraphicsUnit.Pixel;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // mm -> px using device DPI
            float mmToPxX = g.DpiX / 25.4f;
            float mmToPxY = g.DpiY / 25.4f;

            // Printable area
            float printableW = e.MarginBounds.Width;
            float printableH = e.MarginBounds.Height;

            float leftMm = state.Settings.MarginLeftMm;
            float topMm = state.Settings.MarginTopMm;

            float left = leftMm * mmToPxX;
            float top = topMm * mmToPxY;

            const float outerPad = 6f;  // small safety pad inside each cell
            const float rightPad = 8f;  // page-level safety
            const float bottomPad = 8f;

            int perRow = Math.Max(1, state.PerRow);

            float usableW = Math.Max(1f, printableW - left - rightPad);
            float cellW = usableW / perRow;

            // Cell height: barcode + fixed text stack headroom
            float cellH = Math.Max(state.Settings.BarcodeHeight + 90, 180);

            int rowsPerPage = Math.Max(1, (int)((printableH - top - bottomPad) / cellH));

            using var fontTitle = new Font("Segoe UI", 9, FontStyle.Bold);   // shop name smaller
            using var font = new Font("Segoe UI", 8, FontStyle.Regular);

            using var sfCenter = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };
            using var sfLeft = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            for (int r = 0; r < rowsPerPage; r++)
            {
                for (int c = 0; c < perRow; c++)
                {
                    if (state.Index >= state.Items.Count)
                    {
                        e.HasMorePages = false;
                        return;
                    }

                    var item = state.Items[state.Index++];

                    float cellX = e.MarginBounds.Left + left + (c * cellW);
                    float cellY = e.MarginBounds.Top + top + (r * cellH);

                    float innerX = cellX + outerPad;
                    float innerY = cellY + outerPad;
                    float innerW = cellW - (outerPad * 2);

                    // Shop name (centered)
                    var rect = new RectangleF(innerX, innerY, innerW, fontTitle.GetHeight(g));
                    g.DrawString(item.ShopName, fontTitle, Brushes.Black, rect, sfCenter);
                    innerY += rect.Height + 2;

                    // Product + size/color (left)
                    var details = $"{item.ProductName}  {item.Size} {item.Color}".Trim();
                    rect = new RectangleF(innerX, innerY, innerW, font.GetHeight(g));
                    g.DrawString(details, font, Brushes.Black, rect, sfLeft);
                    innerY += rect.Height + 2;

                    // Barcode (centered, 1:1 for crispness)
                    int maxBarcodeWidth = Math.Max(40, (int)Math.Floor(innerW));
                    int desiredWidth = Math.Min(state.Settings.BarcodeWidth, (int)(maxBarcodeWidth * 0.9f)); // slightly smaller
                    int desiredHeight = state.Settings.BarcodeHeight;

                    using var barcodeBmp = CreateBarcodeBitmap(item.Barcode, desiredWidth, desiredHeight);
                    if (barcodeBmp != null)
                    {
                        int bx = (int)(innerX + (innerW - barcodeBmp.Width) / 2f);

                        var oldInterp = g.InterpolationMode;
                        var oldSmooth = g.SmoothingMode;
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.SmoothingMode = SmoothingMode.None;

                        g.DrawImageUnscaled(barcodeBmp, bx, (int)innerY);

                        g.InterpolationMode = oldInterp;
                        g.SmoothingMode = oldSmooth;

                        innerY += barcodeBmp.Height + 4;
                    }

                    // Human-readable barcode (centered)
                    rect = new RectangleF(innerX, innerY, innerW, font.GetHeight(g));
                    g.DrawString(item.Barcode, font, Brushes.Black, rect, sfCenter);
                    innerY += rect.Height + 2;

                    // Price (left)
                    rect = new RectangleF(innerX, innerY, innerW, font.GetHeight(g));
                    g.DrawString($"Price: {item.Price:0}", font, Brushes.Black, rect, sfLeft);
                    innerY += rect.Height + 2;

                    // Secret code (left, optional)
                    if (!string.IsNullOrWhiteSpace(item.SecretCode))
                    {
                        rect = new RectangleF(innerX, innerY, innerW, font.GetHeight(g));
                        g.DrawString($"Code: {item.SecretCode}", font, Brushes.Black, rect, sfLeft);
                    }
                }
            }

            e.HasMorePages = state.Index < state.Items.Count;
        }

        private static Bitmap? CreateBarcodeBitmap(string text, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = Math.Max(80, width),
                    Height = Math.Max(40, height),
                    Margin = 0,
                    PureBarcode = true
                }
            };

            var pixelData = writer.Write(text);

            var bmp = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.WriteOnly,
                bmp.PixelFormat);

            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bmpData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }

            return bmp;
        }

        private static string? ResolvePrinter(string? requested)
        {
            if (string.IsNullOrWhiteSpace(requested)) return null;

            // Exact (case-insensitive) match
            foreach (string p in PrinterSettings.InstalledPrinters)
            {
                if (string.Equals(p, requested, StringComparison.OrdinalIgnoreCase))
                    return p;
            }

            // Normalized (ignore spaces) match
            static string Normalize(string s) =>
                new string(s.Where(ch => !char.IsWhiteSpace(ch)).ToArray()).ToUpperInvariant();

            string target = Normalize(requested);
            foreach (string p in PrinterSettings.InstalledPrinters)
            {
                if (Normalize(p) == target) return p;
            }

            // No match -> null (will fall back to default)
            return null;
        }

        private static void ApplyPaperSize(PrintDocument doc, float widthMm, float heightMm)
        {
            int wHund = (int)Math.Round((widthMm / 25.4f) * 100f);
            int hHund = (int)Math.Round((heightMm / 25.4f) * 100f);

            // Try find a supported paper within 1 hundredth of an inch tolerance
            var match = doc.PrinterSettings.PaperSizes
                .Cast<PaperSize>()
                .FirstOrDefault(ps => Math.Abs(ps.Width - wHund) <= 1 && Math.Abs(ps.Height - hHund) <= 1);

            if (match != null)
            {
                doc.DefaultPageSettings.PaperSize = match;
                return;
            }

            // Use custom paper (Kind = Custom / RawKind = 256)
            var custom = new PaperSize("Custom", wHund, hHund);
            doc.DefaultPageSettings.PaperSize = custom;
        }
    }
}