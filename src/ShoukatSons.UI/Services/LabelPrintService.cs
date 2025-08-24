// ─────────────────────────────────────────────────────
// File: Services/LabelPrintService.cs
// ─────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ShoukatSons.UI.Services.Interfaces;

// Aliases to eliminate ambiguity
using WpfBrushes     = System.Windows.Media.Brushes;
using WpfFlowDir     = System.Windows.FlowDirection;
using WpfPoint       = System.Windows.Point;
using WpfSize        = System.Windows.Size;
using WpfPrintDialog = System.Windows.Controls.PrintDialog;
using WpfWrapPanel   = System.Windows.Controls.WrapPanel;
using WpfOrientation = System.Windows.Controls.Orientation;
using WpfImageCtrl   = System.Windows.Controls.Image;

namespace ShoukatSons.UI.Services
{
    public class LabelPrintService : ILabelPrintService
    {
        public BitmapSource RenderPreview(LabelItemModel item, LabelSettingsModel settings)
        {
            const double dpi    = 96.0;
            const int    width  = 300;
            const int    height = 150;

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawRectangle(
                    WpfBrushes.White,
                    null,
                    new Rect(0, 0, width, height));

                var ft1 = new FormattedText(
                    item.ProductName,
                    System.Globalization.CultureInfo.CurrentCulture,
                    WpfFlowDir.LeftToRight,
                    new Typeface("Segoe UI"),
                    16,
                    WpfBrushes.Black,
                    dpi);

                var ft2 = new FormattedText(
                    $"Rs {item.Price:N0}",
                    System.Globalization.CultureInfo.CurrentCulture,
                    WpfFlowDir.LeftToRight,
                    new Typeface("Segoe UI Semibold"),
                    18,
                    WpfBrushes.Black,
                    dpi);

                var ft3 = new FormattedText(
                    item.Barcode,
                    System.Globalization.CultureInfo.CurrentCulture,
                    WpfFlowDir.LeftToRight,
                    new Typeface("Consolas"),
                    12,
                    WpfBrushes.Black,
                    dpi);

                dc.DrawText(ft1, new WpfPoint(8,  8));
                dc.DrawText(ft2, new WpfPoint(8, 36));
                dc.DrawText(ft3, new WpfPoint(8, 64));
            }

            var rtb = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);
            rtb.Render(dv);
            rtb.Freeze();
            return rtb;
        }

        public void PrintLabels(
            IEnumerable<LabelItemModel> items,
            LabelSettingsModel settings,
            int perRow)
        {
            var labels = items.ToList();
            if (!labels.Any())
                return;

            // Use only WPF PrintDialog
            var dlg = new WpfPrintDialog();
            if (dlg.ShowDialog() != true)
                return;

            var doc      = new FixedDocument();
            var pageSize = new WpfSize(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);

            int i = 0;
            while (i < labels.Count)
            {
                var pageContent = new PageContent();
                var fixedPage   = new FixedPage
                {
                    Width  = pageSize.Width,
                    Height = pageSize.Height
                };

                var panel = new WpfWrapPanel
                {
                    Orientation = WpfOrientation.Horizontal,
                    ItemWidth   = pageSize.Width / Math.Max(1, perRow),
                    ItemHeight  = 160,
                    Margin      = new Thickness(20)
                };

                for (int r = 0; r < perRow && i < labels.Count; r++, i++)
                {
                    var img = new WpfImageCtrl
                    {
                        Source  = RenderPreview(labels[i], settings),
                        Stretch = Stretch.None,
                        Margin  = new Thickness(8)
                    };
                    panel.Children.Add(img);
                }

                fixedPage.Children.Add(panel);
                ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
                doc.Pages.Add(pageContent);
            }

            dlg.PrintDocument(doc.DocumentPaginator, "Product Labels");
        }
    }
}