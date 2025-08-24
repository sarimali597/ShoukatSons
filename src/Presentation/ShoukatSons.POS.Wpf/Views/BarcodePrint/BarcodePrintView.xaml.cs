using System.Windows.Controls;
using ShoukatSons.POS.Wpf.ViewModels.BarcodePrint;

namespace ShoukatSons.POS.Wpf.Views.BarcodePrint
{
    public partial class BarcodePrintView : UserControl
    {
        public BarcodePrintView(BarcodePrintViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}