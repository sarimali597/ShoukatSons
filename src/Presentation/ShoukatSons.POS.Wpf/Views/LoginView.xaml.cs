using System.Windows.Controls;

namespace ShoukatSons.POS.Wpf.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();

            // Bind PasswordBox.Password to ViewModel.Password manually (since PasswordBox isn’t bindable by default)
            PasswordBox.PasswordChanged += (_, __) =>
            {
                if (DataContext is ShoukatSons.POS.Wpf.ViewModels.LoginViewModel vm)
                {
                    vm.Password = PasswordBox.Password;
                }
            };
        }
    }
}