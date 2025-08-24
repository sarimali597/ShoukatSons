using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace ShoukatSons.Core.Models
{
    public class LabelSettings : INotifyPropertyChanged
    {
        private string _printerName = "";
        private float _paperWidthMm = 80f;
        private float _paperHeightMm = 50f;

        // Existing per-edge margins
        private float _marginTopMm = 2f;
        private float _marginLeftMm = 2f;

        // Unified margin (used by LabelComposer)
        private float _marginMm = 2f;

        // Pixel-based barcode sizing
        private int _barcodeWidth = 200;
        private int _barcodeHeight = 50;

        // mm-based barcode height (used by LabelComposer)
        private float _barcodeHeightMm = 12f;

        private int _copies = 1;
        private bool _cloudSyncEnabled = false;

        // Added to satisfy LabelComposer
        private string _alignment = "Left";      // Left, Center, Right
        private bool _showBarcode = true;
        private bool _showName = true;
        private bool _showSize = true;
        private bool _showColor = true;
        private bool _showPrice = true;

        private string _fontFamily = "Segoe UI";
        private double _fontSizePt = 9d;

        // Nullable event to match INotifyPropertyChanged signature
        public event PropertyChangedEventHandler? PropertyChanged;

        public string PrinterName
        {
            get => _printerName;
            set { _printerName = value; OnPropertyChanged(nameof(PrinterName)); }
        }

        public float PaperWidthMm
        {
            get => _paperWidthMm;
            set { _paperWidthMm = value; OnPropertyChanged(nameof(PaperWidthMm)); }
        }

        public float PaperHeightMm
        {
            get => _paperHeightMm;
            set { _paperHeightMm = value; OnPropertyChanged(nameof(PaperHeightMm)); }
        }

        public float MarginTopMm
        {
            get => _marginTopMm;
            set { _marginTopMm = value; OnPropertyChanged(nameof(MarginTopMm)); }
        }

        public float MarginLeftMm
        {
            get => _marginLeftMm;
            set { _marginLeftMm = value; OnPropertyChanged(nameof(MarginLeftMm)); }
        }

        public float MarginMm
        {
            get => _marginMm;
            set { _marginMm = value; OnPropertyChanged(nameof(MarginMm)); }
        }

        public int BarcodeWidth
        {
            get => _barcodeWidth;
            set { _barcodeWidth = value; OnPropertyChanged(nameof(BarcodeWidth)); }
        }

        public int BarcodeHeight
        {
            get => _barcodeHeight;
            set { _barcodeHeight = value; OnPropertyChanged(nameof(BarcodeHeight)); }
        }

        public float BarcodeHeightMm
        {
            get => _barcodeHeightMm;
            set { _barcodeHeightMm = value; OnPropertyChanged(nameof(BarcodeHeightMm)); }
        }

        public int Copies
        {
            get => _copies;
            set { _copies = value; OnPropertyChanged(nameof(Copies)); }
        }

        public bool CloudSyncEnabled
        {
            get => _cloudSyncEnabled;
            set { _cloudSyncEnabled = value; OnPropertyChanged(nameof(CloudSyncEnabled)); }
        }

        public string Alignment
        {
            get => _alignment;
            set { _alignment = value; OnPropertyChanged(nameof(Alignment)); }
        }

        public bool ShowBarcode
        {
            get => _showBarcode;
            set { _showBarcode = value; OnPropertyChanged(nameof(ShowBarcode)); }
        }

        public bool ShowName
        {
            get => _showName;
            set { _showName = value; OnPropertyChanged(nameof(ShowName)); }
        }

        public bool ShowSize
        {
            get => _showSize;
            set { _showSize = value; OnPropertyChanged(nameof(ShowSize)); }
        }

        public bool ShowColor
        {
            get => _showColor;
            set { _showColor = value; OnPropertyChanged(nameof(ShowColor)); }
        }

        public bool ShowPrice
        {
            get => _showPrice;
            set { _showPrice = value; OnPropertyChanged(nameof(ShowPrice)); }
        }

        public string FontFamily
        {
            get => _fontFamily;
            set { _fontFamily = value; OnPropertyChanged(nameof(FontFamily)); }
        }

        public double FontSizePt
        {
            get => _fontSizePt;
            set { _fontSizePt = value; OnPropertyChanged(nameof(FontSizePt)); }
        }

        private void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public static LabelSettings Load(string path)
        {
            if (!File.Exists(path)) return new LabelSettings();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<LabelSettings>(json) ?? new LabelSettings();
        }

        public void Save(string path)
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }
}