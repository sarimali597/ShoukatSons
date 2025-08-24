namespace ShoukatSons.UI.Services.Interfaces
{
    public class LabelSettingsModel
    {
        public double PaperWidthMm { get; set; } = 50;
        public double PaperHeightMm { get; set; } = 30;
        public double MarginTopMm { get; set; } = 2;
        public double MarginLeftMm { get; set; } = 2;
        public int BarcodeWidth { get; set; } = 200;
        public int BarcodeHeight { get; set; } = 60;
        public int Copies { get; set; } = 1;
        public string PrinterName { get; set; } = "";
    }

    public interface ILabelSettingsService
    {
        LabelSettingsModel Load();
        void Save(LabelSettingsModel settings);
        string SettingsSummary(LabelSettingsModel s);
    }
}