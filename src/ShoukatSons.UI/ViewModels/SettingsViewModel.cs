using CommunityToolkit.Mvvm.ComponentModel;

namespace ShoukatSons.UI.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty] private string message = "Settings Screen";
    }
}