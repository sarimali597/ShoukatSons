// src/ShoukatSons.UI/Views/MainWindow.xaml.cs
using System.Windows;
using ShoukatSons.UI.Views;

namespace ShoukatSons.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the user clicks the "Open Barcode Print" button.
        /// Launches the BarcodePrintView as a modal dialog.
        /// </summary>
        private void OpenBarcodePrint_Click(object sender, RoutedEventArgs e)
        {
            var printWindow = new BarcodePrintView
            {
                Owner = this
            };
            printWindow.ShowDialog();
        }
    }
}