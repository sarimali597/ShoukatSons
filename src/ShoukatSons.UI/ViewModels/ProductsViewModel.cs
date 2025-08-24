// ─────────────────────────────────────────────────────
// File: ViewModels/ProductsViewModel.cs
// ─────────────────────────────────────────────────────
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoukatSons.UI.Services.Interfaces;

namespace ShoukatSons.UI.ViewModels
{
    public class ProductListItemDto
    {
        public int    Id            { get; set; }
        public string Barcode       { get; set; } = "";
        public string Name          { get; set; } = "";
        public string CategoryName  { get; set; } = "";
        public decimal PurchasePrice{ get; set; }
        public decimal SalePrice    { get; set; }
        public int    StockQuantity { get; set; }
        public string? Size         { get; set; }
        public string? Color        { get; set; }
    }

    public partial class ProductsViewModel : ObservableObject
    {
        private readonly IDialogService _dialog;

        [ObservableProperty]
        private ProductListItemDto? selectedProduct;

        public ObservableCollection<ProductListItemDto> Products { get; } = new();

        public IRelayCommand AddProductCommand    { get; }
        public IRelayCommand EditProductCommand   { get; }
        public IRelayCommand DeleteProductCommand { get; }

        public ProductsViewModel(IDialogService dialog)
        {
            _dialog = dialog;

            AddProductCommand  = new RelayCommand(AddProduct);
            EditProductCommand = new RelayCommand(EditProduct,   () => SelectedProduct != null);
            DeleteProductCommand = new RelayCommand(DeleteProduct, () => SelectedProduct != null);

            // Manually observe SelectedProduct to update CanExecute
            PropertyChanged += OnPropertyChanged;

            // Mock data
            Products.Add(new ProductListItemDto
            {
                Id             = 1,
                Barcode        = "890123",
                Name           = "Blue Shirt",
                CategoryName   = "Shirts",
                PurchasePrice  = 800,
                SalePrice      = 1500,
                StockQuantity  = 12,
                Size           = "L",
                Color          = "Blue"
            });
            Products.Add(new ProductListItemDto
            {
                Id             = 2,
                Barcode        = "990456",
                Name           = "Black Jeans",
                CategoryName   = "Jeans",
                PurchasePrice  = 1200,
                SalePrice      = 2200,
                StockQuantity  = 8,
                Size           = "32",
                Color          = "Black"
            });
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedProduct))
            {
                ((RelayCommand)EditProductCommand).NotifyCanExecuteChanged();
                ((RelayCommand)DeleteProductCommand).NotifyCanExecuteChanged();
            }
        }

        private void AddProduct()
            => _dialog.Info("Add product (stub). Integrate your product form here.", "Products");

        private void EditProduct()
        {
            if (SelectedProduct == null) return;
            _dialog.Info($"Edit product: {SelectedProduct.Name} (stub).", "Products");
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null) return;
            if (_dialog.Confirm($"Delete {SelectedProduct.Name}?", "Confirm Delete"))
            {
                Products.Remove(SelectedProduct);
                SelectedProduct = null;
                _dialog.Info("Deleted (demo).", "Products");
            }
        }
    }
}