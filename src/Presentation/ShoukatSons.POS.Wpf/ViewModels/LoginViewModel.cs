using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Utilities;
using ShoukatSons.POS.Wpf.ViewModels.Base;
using ShoukatSons.POS.Wpf.Services.Session;

namespace ShoukatSons.POS.Wpf.ViewModels
{
    public sealed class LoginViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;
        private readonly ISessionService _session;

        // Temporary in-memory users (move to SQL later)
        private readonly Dictionary<string, (string Password, string Role)> _users = new()
        {
            { "admin", ("admin123", "Admin") },
            { "cashier", ("cashier123", "Cashier") }
        };

        public LoginViewModel(INavigationService navigation, ISessionService session)
        {
            _navigation = navigation;
            _session = session;
            SignInCommand = new RelayCommand(_ => SignIn(), _ => CanSignIn());
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                (SignInCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                (SignInCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand SignInCommand { get; }

        private bool CanSignIn() =>
            !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private void SignIn()
        {
            if (_users.TryGetValue(Username.Trim(), out var u) && Password == u.Password)
            {
                // Use the new method instead of assigning read-only properties
                _session.Start(Username.Trim(), u.Role);

                _navigation.NavigateTo<DashboardViewModel>();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}