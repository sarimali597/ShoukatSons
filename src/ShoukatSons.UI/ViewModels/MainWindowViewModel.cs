// File: src/ShoukatSons.UI/ViewModels/MainWindowViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ShoukatSons.Core.Interfaces;
using ShoukatSons.Core.Models; // ✅ Correct namespace for Product

namespace ShoukatSons.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IProductService _productService;
        private readonly ILogger<MainWindowViewModel> _logger;

        public ObservableCollection<Product> Products { get; } = new();

        public ICollectionView ProductsView { get; }

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        public IAsyncRelayCommand LoadDataCommand { get; }

        public MainWindowViewModel(
            IProductService productService,
            ILogger<MainWindowViewModel> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger         = logger         ?? throw new ArgumentNullException(nameof(logger));

            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.Filter = FilterProducts;

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SearchText))
                    ProductsView.Refresh();
            };

            LoadDataCommand = new AsyncRelayCommand(LoadProductsAsync, canExecute: () => !IsBusy);

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(IsBusy))
                    LoadDataCommand.NotifyCanExecuteChanged();
            };
        }

        private bool FilterProducts(object obj)
        {
            if (obj is not Product p)
                return false;

            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var search = SearchText.Trim();
            return (p.Name?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                   || p.Id.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                   || p.SalePrice.ToString().Contains(search, StringComparison.OrdinalIgnoreCase);
        }

        private async Task LoadProductsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                _logger.LogInformation("Loading product list…");

                var items = await _productService.ListAsync().ConfigureAwait(false)
                           ?? Enumerable.Empty<Product>();

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Products.Clear();
                    foreach (var item in items)
                        Products.Add(item);
                }).Task.ConfigureAwait(false);

                if (Products.Count > 0)
                    _logger.LogInformation("Loaded {Count} products.", Products.Count);
                else
                    _logger.LogWarning("No products found in database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products from service.");
#if DEBUG
                System.Windows.MessageBox.Show(
                    $"Error loading products:\n{ex.Message}",
                    "Load Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
#endif
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}