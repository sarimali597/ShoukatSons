using System;
using System.IO;
using System.Text.Json;
using ShoukatSons.UI.Services.Interfaces;

namespace ShoukatSons.UI.Services
{
    public class JsonLabelSettingsService : ILabelSettingsService
    {
        private readonly string _dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "ShoukatSonsGarments");

        private string PathFile => System.IO.Path.Combine(_dir, "labelsettings.json");

        public LabelSettingsModel Load()
        {
            try
            {
                Directory.CreateDirectory(_dir);
                if (!File.Exists(PathFile)) return new LabelSettingsModel();
                var json = File.ReadAllText(PathFile);
                return JsonSerializer.Deserialize<LabelSettingsModel>(json) ?? new LabelSettingsModel();
            }
            catch
            {
                return new LabelSettingsModel();
            }
        }

        public void Save(LabelSettingsModel settings)
        {
            Directory.CreateDirectory(_dir);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(PathFile, json);
        }

        public string SettingsSummary(LabelSettingsModel s) =>
            $"W:{s.PaperWidthMm}mm H:{s.PaperHeightMm}mm | Margin T:{s.MarginTopMm}mm L:{s.MarginLeftMm}mm | Barcode {s.BarcodeWidth}x{s.BarcodeHeight}px | Copies:{s.Copies}";
    }
}