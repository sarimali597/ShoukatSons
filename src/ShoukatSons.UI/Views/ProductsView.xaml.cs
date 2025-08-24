using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ShoukatSons.UI.ViewModels;

namespace ShoukatSons.UI.Views
{
    public partial class ProductsView : Window
    {
        public ProductsView(ProductsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        public ProductsView() : this(App.Services.GetRequiredService<ProductsViewModel>()) { }
    }
}