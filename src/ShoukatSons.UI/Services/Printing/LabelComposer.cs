// File: src/ShoukatSons.UI/Services/Printing/LabelComposer.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup; // IAddChild
using System.Windows.Media;
using ShoukatSons.Core.Models;

namespace ShoukatSons.UI.Services.Printing
{
    public static class LabelComposer
    {
        private static double MmToDip(double mm) => (mm / 25.4) * 96.0;
        private static double PtToDip(double pt) => (pt / 72.0) * 96.0;

        private static int DipToPixels(double dips, double printerDpi) =>
            Math.Max(1, (int)Math.Round(dips * (printerDpi / 96.0)));

        public static FixedDocument Compose(Product product, LabelSettings ls, double printerDpi = 300)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (ls == null) throw new ArgumentNullException(nameof(ls));

            double pageWidth  = MmToDip(ls.PaperWidthMm);
            double pageHeight = MmToDip(ls.PaperHeightMm);
            double margin     = MmToDip(ls.MarginMm);
            double barcodeH   = MmToDip(ls.BarcodeHeightMm);
            double fontSize   = PtToDip(ls.FontSizePt);

            double pixelsPerDip = 1.0;
            if (System.Windows.Application.Current?.MainWindow != null)
            {
                var dpiInfo = VisualTreeHelper.GetDpi(System.Windows.Application.Current.MainWindow);
                pixelsPerDip = dpiInfo.PixelsPerDip;
            }

            var drawing = new DrawingGroup();
            using (var dc = drawing.Open())
            {
                var typeface = new Typeface(
                    new System.Windows.Media.FontFamily(ls.FontFamily),
                    FontStyles.Normal,
                    FontWeights.Normal,
                    FontStretches.Normal);

                double y = margin;

                if (ls.ShowName && !string.IsNullOrWhiteSpace(product.Name))
                {
                    var ft = new FormattedText(
                        product.Name,
                        CultureInfo.CurrentCulture,
                        System.Windows.FlowDirection.LeftToRight,
                        typeface,
                        fontSize,
                        System.Windows.Media.Brushes.Black,
                        pixelsPerDip);

                    double x = AlignX(ft.Width, pageWidth, margin, ls.Alignment);
                    dc.DrawText(ft, new System.Windows.Point(x, y));
                    y += ft.Height + 2.0;
                }

                if (ls.ShowSize || ls.ShowColor)
                {
                    string sizePart  = ls.ShowSize  ? product.Size  ?? string.Empty : string.Empty;
                    string colorPart = ls.ShowColor ? product.Color ?? string.Empty : string.Empty;
                    string sep = (ls.ShowSize && ls.ShowColor && !string.IsNullOrEmpty(sizePart) && !string.IsNullOrEmpty(colorPart)) ? " | " : string.Empty;
                    string line = $"{sizePart}{sep}{colorPart}";

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var ft = new FormattedText(
                            line,
                            CultureInfo.CurrentCulture,
                            System.Windows.FlowDirection.LeftToRight,
                            typeface,
                            fontSize,
                            System.Windows.Media.Brushes.Black,
                            pixelsPerDip);

                        double x = AlignX(ft.Width, pageWidth, margin, ls.Alignment);
                        dc.DrawText(ft, new System.Windows.Point(x, y));
                        y += ft.Height + 2.0;
                    }
                }

                if (ls.ShowPrice)
                {
                    var ft = new FormattedText(
                        $"Rs {product.SalePrice:0}",
                        CultureInfo.CurrentCulture,
                        System.Windows.FlowDirection.LeftToRight,
                        typeface,
                        fontSize,
                        System.Windows.Media.Brushes.Black,
                        pixelsPerDip);

                    double x = AlignX(ft.Width, pageWidth, margin, ls.Alignment);
                    dc.DrawText(ft, new System.Windows.Point(x, y));
                    y += ft.Height + 2.0;
                }

                if (ls.ShowBarcode && !string.IsNullOrWhiteSpace(product.Barcode))
                {
                    double barcodeWidthDip = Math.Max(0, pageWidth - (margin * 2));
                    int barcodeWidthPx  = DipToPixels(barcodeWidthDip, printerDpi);
                    int barcodeHeightPx = DipToPixels(barcodeH,       printerDpi);

                    var bmp = BarcodeGenerator.Code128(product.Barcode, barcodeWidthPx, barcodeHeightPx);
                    var rect = new Rect(margin, y, barcodeWidthDip, barcodeH);
                    dc.DrawImage(bmp, rect);
                    y += barcodeH + 1.0;
                }
            }

            var image = new System.Windows.Controls.Image
            {
                Width = pageWidth,
                Height = pageHeight,
                Source = new DrawingImage(drawing)
            };

            var fixedPage = new FixedPage { Width = pageWidth, Height = pageHeight };
            fixedPage.Children.Add(image);
            FixedPage.SetLeft(image, 0);
            FixedPage.SetTop(image, 0);

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);

            var doc = new FixedDocument();
            doc.DocumentPaginator.PageSize = new System.Windows.Size(pageWidth, pageHeight);
            doc.Pages.Add(pageContent);

            return doc;
        }

        private static double AlignX(double elementWidth, double totalWidth, double margin, string alignment)
        {
            return alignment switch
            {
                "Left"  => margin,
                "Right" => Math.Max(margin, totalWidth - margin - elementWidth),
                _       => Math.Max(margin, (totalWidth - elementWidth) / 2.0)
            };
        }
    }
}