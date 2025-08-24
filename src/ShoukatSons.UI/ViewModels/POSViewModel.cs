using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoukatSons.Core.POS;
using ShoukatSons.UI.Services.Interfaces;

namespace ShoukatSons.UI.ViewModels
{
    public partial class CartItemDto : ObservableObject
    {
        [ObservableProperty] private string barcode = "";
        [ObservableProperty] private string name = "";
        [ObservableProperty] private int quantity;
        [ObservableProperty] private decimal price;

        public decimal Total => Price * Quantity;
    }

    public partial class POSViewModel : ObservableObject
    {
        private readonly IProductLookupService _lookup;
        private readonly IDialogService        _dialog;
        private readonly IWindowService        _windowService;

        [ObservableProperty] private string? barcode;
        public ObservableCollection<CartItemDto> CartItems { get; } = new();

        public decimal Total => CartItems.Sum(x => x.Total);

        public IAsyncRelayCommand AddItemCommand    { get; }
        public IRelayCommand      CheckoutCommand   { get; }
        public IRelayCommand      CloseCommand      { get; }

        public POSViewModel(
            IProductLookupService lookup,
            IDialogService        dialog,
            IWindowService        windowService)
        {
            _lookup        = lookup;
            _dialog        = dialog;
            _windowService = windowService;

            CartItems.CollectionChanged += (_, __) => OnPropertyChanged(nameof(Total));

            AddItemCommand  = new AsyncRelayCommand(AddItemAsync);
            CheckoutCommand = new RelayCommand(Checkout);
            CloseCommand    = new RelayCommand(OnClose);
        }

        private async Task AddItemAsync()
        {
            var code = Barcode?.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                _dialog.Warn("Enter or scan a barcode first.", "POS");
                return;
            }

            var product = await _lookup.GetByBarcodeAsync(code, CancellationToken.None);
            if (product == null)
            {
                _dialog.Info("No product found for this barcode.", "POS");
                return;
            }

            var existing = CartItems.FirstOrDefault(x => x.Barcode == product.Barcode);
            if (existing != null)
            {
                existing.Quantity += 1;
                OnPropertyChanged(nameof(Total));
            }
            else
            {
                CartItems.Add(new CartItemDto
                {
                    Barcode  = product.Barcode,
                    Name     = product.Name,
                    Quantity = 1,
                    Price    = product.SalePrice
                });
            }

            Barcode = string.Empty;
        }

        private void Checkout()
        {
            if (!CartItems.Any())
            {
                _dialog.Warn("Cart is empty.", "POS");
                return;
            }

            _dialog.Info(
                $"Checkout complete. Items: {CartItems.Count}, Total: {Total:N2}", 
                "POS");

            CartItems.Clear();
        }

        private void OnClose()
        {
            _windowService.CloseWindow(this);
        }
    }
}