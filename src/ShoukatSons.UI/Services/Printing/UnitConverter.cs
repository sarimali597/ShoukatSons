// src/ShoukatSons.UI/Services/Printing/UnitConverter.cs
using System;

namespace ShoukatSons.UI.Services.Printing
{
    public static class UnitConverter
    {
        /// <summary>
        /// mm to WPF DIPs (1 DIP = 1/96 inch), no snapping.
        /// </summary>
        public static double MmToDip(double mm) => mm * 96.0 / 25.4;

        /// <summary>
        /// mm to integer printer pixels at given DPI.
        /// </summary>
        public static int MmToPixels(double mm, double dpi) =>
            (int)Math.Round(mm * dpi / 25.4, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Printer pixels to mm.
        /// </summary>
        public static double PixelsToMm(double px, double dpi) =>
            px * 25.4 / dpi;

        /// <summary>
        /// Printer pixels to WPF DIPs.
        /// </summary>
        public static double PixelsToDip(double px, double dpi) =>
            px * (96.0 / dpi);

        /// <summary>
        /// Snap a mm value to nearest printer pixel, then return as DIPs.
        /// </summary>
        public static double MmToSnappedDip(double mm, double dpi)
        {
            if (mm <= 0) return 0;
            var px = MmToPixels(mm, dpi);
            return PixelsToDip(px, dpi);
        }

        /// <summary>
        /// Snap an existing DIP value to nearest printer pixel boundary.
        /// </summary>
        public static double SnapDip(double dip, double dpi)
        {
            var px = dip / (96.0 / dpi);
            return PixelsToDip(Math.Round(px), dpi);
        }
    }
}