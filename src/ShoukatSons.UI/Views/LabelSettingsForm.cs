// File: src/ShoukatSons.UI/Views/LabelSettingsForm.cs
using System;
using System.IO;
using System.Threading.Tasks;
using WF = System.Windows.Forms; // Alias WinForms to avoid WPF name conflicts
using ShoukatSons.Core.Models;
using ShoukatSons.Core.Helpers;
using ShoukatSons.Services;
using ShoukatSons.UI.Services.Printing;

namespace ShoukatSons.UI.Views
{
    // Explicit WinForms base class
    public class LabelSettingsForm : WF.Form
    {
        private readonly string _settingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "ShoukatSons", "labelsettings.json");

        private LabelSettings _settings = new LabelSettings();

        // Controls (explicit WinForms types via alias)
        private readonly WF.TextBox txtPrinter = new WF.TextBox { Width = 250 };
        private readonly WF.NumericUpDown numW  = new WF.NumericUpDown { Minimum = 10, Maximum = 200, DecimalPlaces = 1, Increment = 0.5M, Value = 80 };
        private readonly WF.NumericUpDown numH  = new WF.NumericUpDown { Minimum = 10, Maximum = 200, DecimalPlaces = 1, Increment = 0.5M, Value = 50 };
        private readonly WF.NumericUpDown numMT = new WF.NumericUpDown { Minimum = 0,  Maximum = 20,  DecimalPlaces = 1, Increment = 0.5M, Value = 2 };
        private readonly WF.NumericUpDown numML = new WF.NumericUpDown { Minimum = 0,  Maximum = 20,  DecimalPlaces = 1, Increment = 0.5M, Value = 2 };
        private readonly WF.NumericUpDown numBW = new WF.NumericUpDown { Minimum = 50, Maximum = 600, Value = 200 };
        private readonly WF.NumericUpDown numBH = new WF.NumericUpDown { Minimum = 20, Maximum = 300, Value = 50 };
        private readonly WF.NumericUpDown numCopies = new WF.NumericUpDown { Minimum = 1, Maximum = 100, Value = 1 };
        private readonly WF.CheckBox chkCloud = new WF.CheckBox { Text = "Cloud Sync Enable" };
        private readonly WF.Button btnSave = new WF.Button { Text = "Save Settings", Width = 140 };
        private readonly WF.Button btnPrint = new WF.Button { Text = "Print Sample", Width = 140 };

        public LabelSettingsForm()
        {
            Text = "Shoukat Sons - Label Settings";
            Width = 520;
            Height = 420;
            StartPosition = WF.FormStartPosition.CenterScreen;

            var table = new WF.TableLayoutPanel
            {
                Dock = WF.DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 10,
                Padding = new WF.Padding(10)
            };
            table.ColumnStyles.Add(new WF.ColumnStyle(WF.SizeType.Percent, 40));
            table.ColumnStyles.Add(new WF.ColumnStyle(WF.SizeType.Percent, 60));

            void Row(string label, WF.Control c)
            {
                table.Controls.Add(new WF.Label
                {
                    Text = label,
                    AutoSize = true,
                    Anchor = WF.AnchorStyles.Left,
                    Padding = new WF.Padding(0, 6, 0, 0)
                });
                table.Controls.Add(c);
            }

            Row("Printer Name:", txtPrinter);
            Row("Paper Width (mm):", numW);
            Row("Paper Height (mm):", numH);
            Row("Margin Top (mm):", numMT);
            Row("Margin Left (mm):", numML);
            Row("Barcode Width (px):", numBW);
            Row("Barcode Height (px):", numBH);
            Row("Copies:", numCopies);
            Row("", chkCloud);

            var actions = new WF.FlowLayoutPanel
            {
                FlowDirection = WF.FlowDirection.LeftToRight,
                Dock = WF.DockStyle.Fill
            };
            actions.Controls.Add(btnSave);
            actions.Controls.Add(btnPrint);
            table.Controls.Add(new WF.Label());
            table.Controls.Add(actions);

            Controls.Add(table);

            Load += OnLoad;
            btnSave.Click += OnSave;
            btnPrint.Click += OnPrint;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_settingsFile)!);
            _settings = LabelSettings.Load(_settingsFile);

            txtPrinter.Text = _settings.PrinterName;
            numW.Value = (decimal)_settings.PaperWidthMm;
            numH.Value = (decimal)_settings.PaperHeightMm;
            numMT.Value = (decimal)_settings.MarginTopMm;
            numML.Value = (decimal)_settings.MarginLeftMm;
            numBW.Value = _settings.BarcodeWidth;
            numBH.Value = _settings.BarcodeHeight;
            numCopies.Value = _settings.Copies;
            chkCloud.Checked = _settings.CloudSyncEnabled;
        }

        private async void OnSave(object? sender, EventArgs e)
        {
            try
            {
                _settings.PrinterName = txtPrinter.Text;
                _settings.PaperWidthMm = (float)numW.Value;
                _settings.PaperHeightMm = (float)numH.Value;
                _settings.MarginTopMm = (float)numMT.Value;
                _settings.MarginLeftMm = (float)numML.Value;
                _settings.BarcodeWidth = (int)numBW.Value;
                _settings.BarcodeHeight = (int)numBH.Value;
                _settings.Copies = (int)numCopies.Value;
                _settings.CloudSyncEnabled = chkCloud.Checked;

                _settings.Save(_settingsFile);
                BackupManager.CreateBackup(_settingsFile);

                if (_settings.CloudSyncEnabled)
                    await SettingsSync.PushAsync(_settingsFile);

                WF.MessageBox.Show("Settings saved.", "Success", WF.MessageBoxButtons.OK, WF.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                WF.MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void OnPrint(object? sender, EventArgs e)
        {
            try
            {
                LabelPrinter.PrintBarcode("123456789012", _settings);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                WF.MessageBox.Show("Print failed: " + ex.Message);
            }
        }
    }
}