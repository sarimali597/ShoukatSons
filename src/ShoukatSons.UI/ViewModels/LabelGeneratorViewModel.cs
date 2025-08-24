using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoukatSons.UI.Services.Interfaces;

namespace ShoukatSons.UI.ViewModels
{
    public partial class LabelGeneratorViewModel : ObservableObject
    {
        private readonly ILabelSettingsService _settings;
        private readonly ILabelPrintService _printer;
        private readonly IProductLookupService _lookup;
        private readonly IDialogService _dialog;

        [ObservableProperty] private string barcode = "";
        [ObservableProperty] private string name = "";
        [ObservableProperty] private string? size = "";
        [ObservableProperty] private string? color = "";
        [ObservableProperty] private decimal price = 0;
        [ObservableProperty] private string shopName = "ShoukatSons";
        [ObservableProperty] private string? secretCode = "";
        [ObservableProperty] private int totalLabels = 1;
        [ObservableProperty] private int labelsPerRow = 3;
        [ObservableProperty] private BitmapSource? previewImage;

        public ObservableCollection<string> ShopOptions { get; } =
            new(new[] { "ShoukatSons", "ShoukatSonsGarments" });

        [ObservableProperty] private LabelSettingsModel currentSettings;
        [ObservableProperty] private string settingsSummary = "";

        public IAsyncRelayCommand FetchCommand { get; }
        public IRelayCommand ClearAllCommand { get; }
        public IRelayCommand PreviewCommand { get; }
        public IRelayCommand PrintCommand { get; }
        public IRelayCommand EditSettingsCommand { get; }
        public IRelayCommand CloseCommand { get; }

        public LabelGeneratorViewModel(ILabelSettingsService settings, ILabelPrintService printer, IProductLookupService lookup, IDialogService dialog)
        {
            _settings = settings;
            _printer = printer;
            _lookup = lookup;
            _dialog = dialog;

            CurrentSettings = _settings.Load();
            SettingsSummary = _settings.SettingsSummary(CurrentSettings);

            FetchCommand = new AsyncRelayCommand(FetchAsync);
            ClearAllCommand = new RelayCommand(ClearAll);
            PreviewCommand = new RelayCommand(Preview);
            PrintCommand = new RelayCommand(Print);
            EditSettingsCommand = new RelayCommand(EditSettings);
            CloseCommand = new RelayCommand(() => { /* handled by window */ });
        }

        private async Task FetchAsync()
        {
            var code = Barcode?.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                _dialog.Warn("Enter a barcode number first.", "Label Generator");
                return;
            }

            var ct = CancellationToken.None;
            var product = await _lookup.GetByBarcodeAsync(code, ct);
            if (product == null)
            {
                _dialog.Info("No product found for this barcode.", "Label Generator");
                return;
            }

            Name = product.Name;
            Size = product.Size;
            Color = product.Color;
            Price = product.SalePrice;
        }

        private void Preview()
        {
            var item = BuildItem();
            if (item == null) return;
            PreviewImage = _printer.RenderPreview(item, CurrentSettings);
        }

        private void Print()
        {
            var item = BuildItem();
            if (item == null) return;

            if (TotalLabels < 1) TotalLabels = 1;
            if (LabelsPerRow < 1) LabelsPerRow = 3;

            var items = Enumerable.Repeat(item, TotalLabels).ToList();
            _printer.PrintLabels(items, CurrentSettings, LabelsPerRow);
            _dialog.Info("Sent to printer.", "Label Generator");
        }

        private void EditSettings()
        {
            // Simple toggle demo: increase copies, in real app open window/form
            CurrentSettings.Copies = CurrentSettings.Copies <= 0 ? 1 : CurrentSettings.Copies;
            _settings.Save(CurrentSettings);
            SettingsSummary = _settings.SettingsSummary(CurrentSettings);
        }

        private void ClearAll()
        {
            Barcode = "";
            Name = "";
            Size = "";
            Color = "";
            Price = 0;
            SecretCode = "";
            ShopName = "ShoukatSons";
            TotalLabels = 1;
            LabelsPerRow = 3;
            PreviewImage = null;
        }

        private LabelItemModel? BuildItem()
        {
            var code = Barcode?.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                _dialog.Warn("Barcode is required.", "Label Generator");
                return null;
            }

            return new LabelItemModel(code, Name?.Trim() ?? "", Size?.Trim(), Color?.Trim(), Price, ShopName, SecretCode?.Trim());
        }
    }
}