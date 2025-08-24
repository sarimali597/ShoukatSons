// File: src/ShoukatSons.UI/Views/ProductsView.xaml.cs
using System;
using System.Windows;

namespace ShoukatSons.UI.Views
{
    // Match the XAML root: <Window x:Class="ShoukatSons.UI.Views.ProductsView" ...>
    public partial class ProductsView : Window
    {
        public ProductsView()
        {
            InitializeComponent();
        }

        // Button handlers referenced in XAML so compile-time binding succeeds
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Add clicked", "Products", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Edit clicked", "Products", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var confirm = System.Windows.MessageBox.Show(
                "Delete selected item?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                System.Windows.MessageBox.Show("Deleted (demo)", "Products", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}