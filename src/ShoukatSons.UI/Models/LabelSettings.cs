// src/ShoukatSons.UI/Models/LabelSettings.cs
#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace ShoukatSons.UI.Models
{
    public class LabelSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        //— Core layout (mm)
        private double _rollWidthMm      = 108;
        private double _labelWidthMm     = 32;
        private double _labelHeightMm    = 19;
        private int    _columns          = 2;
        private double _horizontalGapMm  = 2;
        private double _verticalGapMm    = 2;
        private double _columnGapMm      = 2;
        private double _rowGapMm         = 3;
        private double _marginLeftMm     = 2;
        private double _marginTopMm      = 2;
        private double _marginRightMm    = 2;
        private double _marginBottomMm   = 2;
        private double _fontScale        = 1.0;
        private string? _printerName;

        //— Padding (mm)
        private double _paddingLeftMm    = 1;
        private double _paddingTopMm     = 1;
        private double _paddingRightMm   = 1;
        private double _paddingBottomMm  = 1;

        //— Typography (mm)
        private double _shopNameFontMm    = 2.2;
        private double _secretCodeFontMm  = 1.8;
        private double _nameFontMm        = 2.6;
        private double _sizeFontMm        = 2.4;
        private double _priceFontMm       = 3.0;
        private double _skuFontMm         = 2.2;

        //— Visibility
        private bool _showShopName       = true;
        private string _barcodeSymbology = "Code128"; // default

[JsonInclude]
public string BarcodeSymbology
{
    get => _barcodeSymbology;
    set => SetProperty(ref _barcodeSymbology, value);
}
        private bool _showSecretCode     = true;
        private bool _showSize           = true;
        private bool _showName           = true;
        private bool _showPrice          = true;
        private bool _showSku            = true;

        //— Currency & barcode specifics
        private string _currencyPrefix    = "Rs ";
        private double _barcodeHeightMm   = 12;
        private double _barcodeQuietZoneMm= 1;
        private bool   _skuBelowBarcode   = true;
        private double _skuBelowBarcodeGapMm = 0.8;

        //— DPI settings
        private double _targetPrinterDpi = 203;
        private double _screenDpi        = 96;

        [JsonInclude] public double RollWidthMm      { get => _rollWidthMm;      set => SetProperty(ref _rollWidthMm, value); }
        [JsonInclude] public double LabelWidthMm     { get => _labelWidthMm;     set => SetProperty(ref _labelWidthMm, value); }
        [JsonInclude] public double LabelHeightMm    { get => _labelHeightMm;    set => SetProperty(ref _labelHeightMm, value); }
        [JsonInclude] public int    Columns          { get => _columns;          set => SetProperty(ref _columns, Math.Max(1, value)); }
        [JsonInclude] public double HorizontalGapMm  { get => _horizontalGapMm;  set => SetProperty(ref _horizontalGapMm, value); }
        [JsonInclude] public double VerticalGapMm    { get => _verticalGapMm;    set => SetProperty(ref _verticalGapMm, value); }
        [JsonInclude] public double ColumnGapMm      { get => _columnGapMm;      set => SetProperty(ref _columnGapMm, value); }
        [JsonInclude] public double RowGapMm         { get => _rowGapMm;         set => SetProperty(ref _rowGapMm, value); }
        [JsonInclude] public double MarginLeftMm     { get => _marginLeftMm;     set => SetProperty(ref _marginLeftMm, value); }
        [JsonInclude] public double MarginTopMm      { get => _marginTopMm;      set => SetProperty(ref _marginTopMm, value); }
        [JsonInclude] public double MarginRightMm    { get => _marginRightMm;    set => SetProperty(ref _marginRightMm, value); }
        [JsonInclude] public double MarginBottomMm   { get => _marginBottomMm;   set => SetProperty(ref _marginBottomMm, value); }
        [JsonInclude] public double FontScale        { get => _fontScale;        set => SetProperty(ref _fontScale, Math.Max(0.5, Math.Min(2.0, value))); }
        [JsonInclude] public string? PrinterName     { get => _printerName;      set => SetProperty(ref _printerName, value); }

        [JsonInclude] public double PaddingLeftMm    { get => _paddingLeftMm;    set => SetProperty(ref _paddingLeftMm, value); }
        [JsonInclude] public double PaddingTopMm     { get => _paddingTopMm;     set => SetProperty(ref _paddingTopMm, value); }
        [JsonInclude] public double PaddingRightMm   { get => _paddingRightMm;   set => SetProperty(ref _paddingRightMm, value); }
        [JsonInclude] public double PaddingBottomMm  { get => _paddingBottomMm;  set => SetProperty(ref _paddingBottomMm, value); }

        [JsonInclude] public double ShopNameFontMm    { get => _shopNameFontMm;    set => SetProperty(ref _shopNameFontMm, value); }
        [JsonInclude] public double SecretCodeFontMm  { get => _secretCodeFontMm;  set => SetProperty(ref _secretCodeFontMm, value); }
        [JsonInclude] public double NameFontMm        { get => _nameFontMm;        set => SetProperty(ref _nameFontMm, value); }
        [JsonInclude] public double SizeFontMm        { get => _sizeFontMm;        set => SetProperty(ref _sizeFontMm, value); }
        [JsonInclude] public double PriceFontMm       { get => _priceFontMm;       set => SetProperty(ref _priceFontMm, value); }
        [JsonInclude] public double SkuFontMm         { get => _skuFontMm;         set => SetProperty(ref _skuFontMm, value); }

        [JsonInclude] public bool   ShowShopName      { get => _showShopName;      set => SetProperty(ref _showShopName, value); }
        [JsonInclude] public bool   ShowSecretCode    { get => _showSecretCode;    set => SetProperty(ref _showSecretCode, value); }
        [JsonInclude] public bool   ShowSize          { get => _showSize;          set => SetProperty(ref _showSize, value); }
        [JsonInclude] public bool   ShowName          { get => _showName;          set => SetProperty(ref _showName, value); }
        [JsonInclude] public bool   ShowPrice         { get => _showPrice;         set => SetProperty(ref _showPrice, value); }
        [JsonInclude] public bool   ShowSku           { get => _showSku;           set => SetProperty(ref _showSku, value); }

        [JsonInclude] public string CurrencyPrefix    { get => _currencyPrefix;    set => SetProperty(ref _currencyPrefix, value); }
        [JsonInclude] public double BarcodeHeightMm   { get => _barcodeHeightMm;   set => SetProperty(ref _barcodeHeightMm, value); }
        [JsonInclude] public double BarcodeQuietZoneMm{ get => _barcodeQuietZoneMm;set => SetProperty(ref _barcodeQuietZoneMm, value); }
        [JsonInclude] public bool   SkuBelowBarcode   { get => _skuBelowBarcode;   set => SetProperty(ref _skuBelowBarcode, value); }
        [JsonInclude] public double SkuBelowBarcodeGapMm { get => _skuBelowBarcodeGapMm; set => SetProperty(ref _skuBelowBarcodeGapMm, value); }

        [JsonInclude] public double TargetPrinterDpi  { get => _targetPrinterDpi;  set => SetProperty(ref _targetPrinterDpi, value); }
        [JsonInclude] public double ScreenDpi         { get => _screenDpi;         set => SetProperty(ref _screenDpi, value); }

        public void Validate()
        {
            if (Columns < 1) throw new ArgumentOutOfRangeException(nameof(Columns));
            if (LabelWidthMm <= 0) throw new ArgumentOutOfRangeException(nameof(LabelWidthMm));
            if (LabelHeightMm <= 0) throw new ArgumentOutOfRangeException(nameof(LabelHeightMm));
            if (RollWidthMm <= 0) throw new ArgumentOutOfRangeException(nameof(RollWidthMm));
            if (BarcodeHeightMm <= 0) throw new ArgumentOutOfRangeException(nameof(BarcodeHeightMm));
        }

        public double EffectiveLabelWidthMm  => LabelWidthMm;
        public double EffectiveLabelHeightMm => LabelHeightMm;

        public static int MmToPixels(double mm, double dpi)
            => (int)Math.Round(mm * dpi / 25.4);

        protected bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}