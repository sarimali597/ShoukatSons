using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ShoukatSons.UI.Services.Interfaces
{
    public record LabelItemModel(string Barcode, string ProductName, string? Size, string? Color, decimal Price, string ShopName, string? SecretCode);

    public interface ILabelPrintService
    {
        BitmapSource RenderPreview(LabelItemModel item, LabelSettingsModel settings);
        void PrintLabels(IEnumerable<LabelItemModel> items, LabelSettingsModel settings, int perRow);
    }
}