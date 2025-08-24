using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace ShoukatSons.UI.Services.Printing
{
    public static class BarcodeGenerator
    {
        public static BitmapSource Code128(string content, int width = 300, int height = 100)
        {
            var writer = new ZXing.BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions { Width = width, Height = height, Margin = 0, PureBarcode = true }
            };

            var pixelData = writer.Write(content);
            var bmp = BitmapSource.Create(pixelData.Width, pixelData.Height, 96, 96,
                System.Windows.Media.PixelFormats.Gray8, null, pixelData.Pixels, pixelData.Width);
            return bmp;
        }
    }
}