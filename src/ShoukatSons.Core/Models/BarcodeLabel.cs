namespace ShoukatSons.Core.Models
{
    /// <summary>
    /// Complete data model for a printable barcode label.
    /// </summary>
    public sealed class BarcodeLabel
    {
        /// <summary>
        /// Shop display name (e.g., Shoukat Sons Garments).
        /// </summary>
        public string ShopName { get; set; } = string.Empty;

        /// <summary>
        /// Internal code or SKU.
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// Product name/title for the label.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Size text (e.g., M, L, 32x34).
        /// </summary>
        public string Size { get; set; } = string.Empty;

        /// <summary>
        /// Price string including currency prefix if desired (e.g., PKR 1850.00).
        /// </summary>
        public string Price { get; set; } = string.Empty;

        /// <summary>
        /// Raw barcode content to encode (alphanumeric for Code128; digits for EAN-13).
        /// </summary>
        public string Barcode { get; set; } = string.Empty;

        /// <summary>
        /// Chosen barcode symbology.
        /// NOTE: Ean13 casing matches BarcodeGenerator.cs usage.
        /// </summary>
        public BarcodeSymbology Symbology { get; set; } = BarcodeSymbology.Code128;

        /// <summary>
        /// Label width in millimeters.
        /// </summary>
        public double WidthMm { get; set; } = 50;

        /// <summary>
        /// Label height in millimeters.
        /// </summary>
        public double HeightMm { get; set; } = 30;

        /// <summary>
        /// Number of copies to print.
        /// </summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Optional target printer name. If null or empty, default printer is used.
        /// </summary>
        public string? PrinterName { get; set; }
    }

    /// <summary>
    /// Supported barcode types for label generation.
    /// </summary>
    public enum BarcodeSymbology
    {
        Code128,
        Ean13,
        QrCode
    }
}