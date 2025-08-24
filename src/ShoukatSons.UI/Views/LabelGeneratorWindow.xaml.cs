// File: src/ShoukatSons.UI/Views/LabelGeneratorWindow.xaml.cs
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ShoukatSons.Core.Models;
using ShoukatSons.UI.Services.Printing;

namespace ShoukatSons.UI.Views
{
    public partial class LabelGeneratorWindow : Window
    {
        private LabelSettings _settings = new LabelSettings();

        public LabelGeneratorWindow()
        {
            InitializeComponent();
            EnsureSettingsDirectory();
            LoadSettings();
            UpdateSettingsSummary();
            Loaded += (_, __) => TxtBarcode?.Focus();
        }

        private string SettingsPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                         "ShoukatSons", "labelsettings.json");

        private static void EnsureSettingsDirectory()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ShoukatSons");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private void LoadSettings()
        {
            try
            {
                _settings = LabelSettings.Load(SettingsPath);
            }
            catch
            {
                _settings = new LabelSettings();
            }
        }

        private void UpdateSettingsSummary()
        {
            TxtSettingsSummary.Text =
                $"Settings: W:{_settings.PaperWidthMm}mm H:{_settings.PaperHeightMm}mm | " +
                $"Margin T:{_settings.MarginTopMm}mm L:{_settings.MarginLeftMm}mm | " +
                $"Barcode {_settings.BarcodeWidth}x{_settings.BarcodeHeight}px | Copies:{_settings.Copies}";
        }

        private async void Fetch_Click(object? sender, RoutedEventArgs e)
        {
            var code = TxtBarcode.Text?.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                System.Windows.MessageBox.Show("Enter a barcode number first.", "Label Generator",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var product = await StubFetchProductAsync(code);
                if (product == null)
                {
                    System.Windows.MessageBox.Show("No product found for this barcode.", "Label Generator",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                TxtName.Text = product.Name ?? "";
                TxtSize.Text = product.Size ?? "";
                TxtColor.Text = product.Color ?? "";
                TxtPrice.Text = product.SalePrice.ToString("0.##");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Fetch failed: " + ex.Message, "Label Generator",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Replace with your real data source
        private Task<ProductDto?> StubFetchProductAsync(string barcode)
        {
            return Task.FromResult<ProductDto?>(new ProductDto
            {
                Barcode = barcode,
                Name = "Sample Product",
                Size = "L",
                Color = "Black",
                SalePrice = 1499m
            });
        }

        private void EditSettings_Click(object? sender, RoutedEventArgs e)
        {
            var form = new LabelSettingsForm
            {
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            };
            form.ShowDialog();
            LoadSettings();
            UpdateSettingsSummary();
        }

        private void Preview_Click(object? sender, RoutedEventArgs e)
        {
            var item = BuildLabelItem();
            if (item == null) return;

            try
            {
                using var bmp = CustomLabelPrinter.RenderPreviewBitmap(item, _settings);
                if (bmp == null)
                {
                    System.Windows.MessageBox.Show("Preview render failed.", "Label Generator",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    ImgPreview.Source = null;
                    return;
                }
                ImgPreview.Source = ConvertToBitmapSource(bmp);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Preview failed: " + ex.Message, "Label Generator",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                ImgPreview.Source = null;
            }
        }

        private void Print_Click(object? sender, RoutedEventArgs e)
        {
            var item = BuildLabelItem();
            if (item == null) return;

            if (!int.TryParse(TxtTotal.Text.Trim(), out var total) || total < 1)
                total = 1;
            if (!int.TryParse(TxtPerRow.Text.Trim(), out var perRow) || perRow < 1)
                perRow = 3;

            var items = Enumerable.Repeat(item, total).ToList();

            try
            {
                if (string.IsNullOrWhiteSpace(_settings.PrinterName))
                    _settings.PrinterName = "BlackCopper BC-LP1300";

                CustomLabelPrinter.PrintLabels(items, _settings, perRow);
                System.Windows.MessageBox.Show("Sent to printer.", "Label Generator",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Print failed: " + ex.Message, "Label Generator",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearAll_Click(object? sender, RoutedEventArgs e)
        {
            TxtBarcode.Text = "";
            TxtName.Text = "";
            TxtSize.Text = "";
            TxtColor.Text = "";
            TxtPrice.Text = "";
            TxtSecret.Text = "";
            CmbShop.SelectedIndex = 0;
            TxtTotal.Text = "1";
            TxtPerRow.Text = "3";
            ImgPreview.Source = null;
            TxtBarcode.Focus();
        }

        private void Close_Click(object? sender, RoutedEventArgs e) => Close();

        private LabelItem? BuildLabelItem()
        {
            var barcode = TxtBarcode.Text?.Trim();
            if (string.IsNullOrWhiteSpace(barcode))
            {
                System.Windows.MessageBox.Show("Barcode is required.", "Label Generator",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (!decimal.TryParse(TxtPrice.Text?.Trim(), out var price))
                price = 0m;

            var shop = (CmbShop.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString()
                       ?? "ShoukatSons";

            return new LabelItem
            {
                Barcode = barcode,
                ProductName = TxtName.Text?.Trim() ?? "",
                Size = TxtSize.Text?.Trim() ?? "",
                Color = TxtColor.Text?.Trim() ?? "",
                Price = price,
                ShopName = shop,
                SecretCode = TxtSecret.Text?.Trim() ?? ""
            };
        }

        private static BitmapSource ConvertToBitmapSource(System.Drawing.Bitmap bmp)
        {
            using var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze();
            return image;
        }

        private sealed class ProductDto
        {
            public string? Barcode { get; set; }
            public string? Name { get; set; }
            public string? Size { get; set; }
            public string? Color { get; set; }
            public decimal SalePrice { get; set; }
        }
    }
}