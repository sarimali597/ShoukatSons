// File: src/ShoukatSons.UI/Views/POSWindow.xaml.cs
using System;
using System.Windows;

namespace ShoukatSons.UI.Views
{
    public partial class POSWindow : Window
    {
        public POSWindow()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(
                "Add button clicked (placeholder)",
                "POS",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(
                "Checkout clicked (placeholder)",
                "POS",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}