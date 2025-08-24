using System;
using System.Windows;
using ShoukatSons.Core.Models;

namespace ShoukatSons.UI.Views
{
    public partial class ProductEditWindow : Window
    {
        public Product Product { get; private set; }

        public ProductEditWindow()
        {
            InitializeComponent();
            Product = new Product();
        }

        public ProductEditWindow(Product existing) : this()
        {
            Product = existing;
            TxtBarcode.Text = existing.Barcode;
            TxtName.Text = existing.Name;
            TxtPurchasePrice.Text = existing.PurchasePrice.ToString();
            TxtSalePrice.Text = existing.SalePrice.ToString();
            TxtStockQuantity.Text = existing.StockQuantity.ToString();
            TxtSize.Text = existing.Size;
            TxtColor.Text = existing.Color;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Product.Barcode = TxtBarcode.Text.Trim();
            Product.Name = TxtName.Text.Trim();
            Product.PurchasePrice = decimal.TryParse(TxtPurchasePrice.Text, out var pp) ? pp : 0;
            Product.SalePrice = decimal.TryParse(TxtSalePrice.Text, out var sp) ? sp : 0;
            Product.StockQuantity = int.TryParse(TxtStockQuantity.Text, out var sq) ? sq : 0;
            Product.Size = TxtSize.Text.Trim();
            Product.Color = TxtColor.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}