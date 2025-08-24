// src/ShoukatSons.UI/Models/LabelItem.cs
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ShoukatSons.UI.Models
{
    /// <summary>
    /// Strong, WPF-friendly label model with validation, change notification,
    /// and useful computed properties for barcode and price display.
    /// </summary>
    public class LabelItem : INotifyPropertyChanged, IValidatableObject
    {
        private string _shopName = "Shoukat Sons Garments";
        private string? _secretCode;
        private string _productName = string.Empty;
        private string? _size;
        private string _sku = string.Empty;
        private decimal _price;
        private int _quantity = 1;
        private string _currencySymbol = "Rs";
        private bool _showCurrencySymbol = true;

        /// <summary>
        /// Shop name printed on the label.
        /// </summary>
        [Required, StringLength(40, MinimumLength = 2)]
        public string ShopName
        {
            get => _shopName;
            set => SetProperty(ref _shopName, NormalizeText(value, trim: true), nameof(ShopName), nameof(DisplayName));
        }

        /// <summary>
        /// Optional internal or secret code (not necessarily printed).
        /// </summary>
        [StringLength(32)]
        public string? SecretCode
        {
            get => _secretCode;
            set
            {
                if (SetProperty(ref _secretCode, NormalizeText(value, trim: true)))
                {
                    OnPropertyChanged(nameof(BarcodeContent));
                }
            }
        }

        /// <summary>
        /// Product name to print (primary display line).
        /// </summary>
        [Required, StringLength(60, MinimumLength = 1)]
        public string ProductName
        {
            get => _productName;
            set
            {
                if (SetProperty(ref _productName, NormalizeText(value, trim: true)))
                {
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        /// <summary>
        /// Optional size (e.g., S, M, L, 32x34).
        /// </summary>
        [StringLength(16)]
        public string? Size
        {
            get => _size;
            set => SetProperty(ref _size, NormalizeText(value, trim: true));
        }

        /// <summary>
        /// Stock keeping unit. Also used as default barcode value if present.
        /// </summary>
        [Required, StringLength(32, MinimumLength = 2)]
        public string SKU
        {
            get => _sku;
            set
            {
                // Normalize: trim, remove inner spaces, make uppercase.
                var normalized = NormalizeSKU(value);
                if (SetProperty(ref _sku, normalized))
                {
                    OnPropertyChanged(nameof(BarcodeContent));
                }
            }
        }

        /// <summary>
        /// Unit price in PKR.
        /// </summary>
        [Range(0, 1_000_000)]
        public decimal Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value))
                {
                    OnPropertyChanged(nameof(FormattedPrice));
                }
            }
        }

        /// <summary>
        /// Number of labels to print.
        /// </summary>
        [Range(1, 1000)]
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        /// <summary>
        /// Currency symbol prefix for price formatting (default: Rs).
        /// </summary>
        [Required, StringLength(6, MinimumLength = 1)]
        public string CurrencySymbol
        {
            get => _currencySymbol;
            set
            {
                if (SetProperty(ref _currencySymbol, NormalizeText(value, trim: true)))
                {
                    OnPropertyChanged(nameof(FormattedPrice));
                }
            }
        }

        /// <summary>
        /// Toggle whether to show currency symbol in FormattedPrice.
        /// </summary>
        public bool ShowCurrencySymbol
        {
            get => _showCurrencySymbol;
            set
            {
                if (SetProperty(ref _showCurrencySymbol, value))
                {
                    OnPropertyChanged(nameof(FormattedPrice));
                }
            }
        }

        /// <summary>
        /// Created timestamp for auditing/logging (not printed).
        /// </summary>
        public DateTime CreatedAt { get; } = DateTime.Now;

        /// <summary>
        /// Best label display name, trimmed to fit typical label width.
        /// </summary>
        public string DisplayName => Truncate(ProductName, 28);

        /// <summary>
        /// Computed, human-friendly price (e.g., "Rs 1,250" or "1,250").
        /// </summary>
        public string FormattedPrice
        {
            get
            {
                var number = Price % 1 == 0 ? Price.ToString("N0") : Price.ToString("N2");
                return ShowCurrencySymbol ? $"{CurrencySymbol} {number}" : number;
            }
        }

        /// <summary>
        /// Value to encode in the barcode. Prefers SKU; falls back to SecretCode.
        /// </summary>
        public string BarcodeContent => string.IsNullOrWhiteSpace(SKU) ? (SecretCode ?? string.Empty) : SKU;

        /// <summary>
        /// True if the item is ready for printing (quick check).
        /// </summary>
        public bool IsPrintable =>
            !string.IsNullOrWhiteSpace(ProductName) &&
            !string.IsNullOrWhiteSpace(BarcodeContent) &&
            Quantity >= 1 &&
            Price >= 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // DataAnnotations attributes will be honored by external validators.
            // Add cross-field rules here.

            if (string.IsNullOrWhiteSpace(SKU) && string.IsNullOrWhiteSpace(SecretCode))
            {
                yield return new ValidationResult(
                    "Either SKU or SecretCode must be provided for barcode generation.",
                    new[] { nameof(SKU), nameof(SecretCode) });
            }

            if (!string.IsNullOrWhiteSpace(SKU) && SKU.Length < 2)
            {
                yield return new ValidationResult(
                    "SKU is too short.",
                    new[] { nameof(SKU) });
            }

            if (Quantity < 1 || Quantity > 1000)
            {
                yield return new ValidationResult(
                    "Quantity must be between 1 and 1000.",
                    new[] { nameof(Quantity) });
            }

            if (Price < 0)
            {
                yield return new ValidationResult(
                    "Price cannot be negative.",
                    new[] { nameof(Price) });
            }
        }

        /// <summary>
        /// Creates a deep copy for duplicating or batch printing.
        /// </summary>
        public LabelItem Clone() => new LabelItem
        {
            ShopName = ShopName,
            SecretCode = SecretCode,
            ProductName = ProductName,
            Size = Size,
            SKU = SKU,
            Price = Price,
            Quantity = Quantity,
            CurrencySymbol = CurrencySymbol,
            ShowCurrencySymbol = ShowCurrencySymbol
        };

        public override string ToString()
        {
            return $"{DisplayName} | {FormattedPrice} | SKU: {SKU}" +
                   (string.IsNullOrWhiteSpace(Size) ? string.Empty : $" | Size: {Size}");
        }

        // Helpers

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null, params string[] alsoNotify)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);

            if (alsoNotify is { Length: > 0 })
            {
                foreach (var name in alsoNotify)
                    OnPropertyChanged(name);
            }

            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private static string NormalizeText(string? input, bool trim)
        {
            var s = input ?? string.Empty;
            if (trim) s = s.Trim();
            return s;
        }

        private static string NormalizeSKU(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            // Remove spaces and common separators; force uppercase.
            var cleaned = input.Trim()
                               .Replace(" ", string.Empty)
                               .Replace("\t", string.Empty)
                               .Replace("-", string.Empty)
                               .Replace("_", string.Empty);
            return cleaned.ToUpperInvariant();
        }

        private static string Truncate(string value, int max)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= max ? value : value.Substring(0, max).TrimEnd();
        }

        // Convenience factory
        public static LabelItem CreateBasic(string productName, string sku, decimal price, int quantity = 1, string? size = null)
        {
            return new LabelItem
            {
                ProductName = productName,
                SKU = sku,
                Price = price,
                Quantity = quantity,
                Size = size
            };
        }
    }
}