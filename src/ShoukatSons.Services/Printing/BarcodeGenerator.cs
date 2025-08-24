using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Services.Printing
{
    public static class BarcodeGenerator
    {
        public static BitmapSource Generate(BarcodeLabel label, int widthPx, int heightPx)
        {
            var format = label.Symbology switch
            {
                BarcodeSymbology.Ean13 => BarcodeFormat.EAN_13,
                _ => BarcodeFormat.CODE_128
            };

            var options = new EncodingOptions
            {
                Width = widthPx,
                Height = heightPx,
                Margin = 0,
                PureBarcode = true
            };

            var writer = new BarcodeWriterPixelData
            {
                Format = format,
                Options = options
            };

            var content = label.Barcode?.Trim() ?? string.Empty;
            if (format == BarcodeFormat.EAN_13)
            {
                if (content.Length != 12 && content.Length != 13)
                    throw new ArgumentException("EAN-13 requires 12 or 13 digits.");
                if (!long.TryParse(content, out _))
                    throw new ArgumentException("EAN-13 content must be digits only.");
            }

            var pixelData = writer.Write(content);

            // ZXing returns BGRA32. Use matching PixelFormat and stride = width * 4.
            var bmp = BitmapSource.Create(
                pixelData.Width,
                pixelData.Height,
                96, 96,
                PixelFormats.Bgra32,
                null,
                pixelData.Pixels,
                pixelData.Width * 4);

            return bmp;
        }
    }
}