using System;
using System.Drawing;
using System.Drawing.Printing;
using ShoukatSons.Core.Models;
using ZXing;
using ZXing.Common;

namespace ShoukatSons.UI.Services.Printing
{
    public static class LabelPrinter
    {
        private static int MmToHundredthInch(float mm) =>
            (int)(mm / 25.4f * 100f);

        public static void PrintBarcode(string code, LabelSettings settings)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Barcode code is empty.", nameof(code));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            // Generate barcode bitmap
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = Math.Max(1, settings.BarcodeHeight),
                    Width  = Math.Max(1, settings.BarcodeWidth),
                    Margin = 0,
                    PureBarcode = true
                }
            };

            var pixelData = writer.Write(code);
            if (pixelData.Pixels == null || pixelData.Pixels.Length == 0)
                throw new InvalidOperationException("Failed to generate barcode image.");

            using var bmp = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                    bmp.PixelFormat);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, data.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            // Prepare print document
            using var pd = new PrintDocument();

            if (!string.IsNullOrWhiteSpace(settings.PrinterName))
            {
                pd.PrinterSettings.PrinterName = settings.PrinterName;
            }

            pd.DefaultPageSettings.PaperSize = new PaperSize(
                "Custom",
                MmToHundredthInch(settings.PaperWidthMm),
                MmToHundredthInch(settings.PaperHeightMm));

            pd.DefaultPageSettings.Margins = new Margins(
                MmToHundredthInch(settings.MarginLeftMm),
                0,
                MmToHundredthInch(settings.MarginTopMm),
                0);

            int printed = 0;
            pd.PrintPage += (s, e) =>
            {
                var g = e.Graphics;
                if (g != null)
                {
                    g.DrawImage(bmp, 0, 0);
                }

                printed++;
                e.HasMorePages = printed < Math.Max(1, settings.Copies);
            };

            pd.Print();
        }
    }
}