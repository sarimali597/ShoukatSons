param(
  [Parameter(Mandatory)]
  [string]$Name
)

$vmDir   = "src\Presentation\ShoukatSons.POS.Wpf\ViewModels"
$viewDir = "src\Presentation\ShoukatSons.POS.Wpf\Views"

# Generate ViewModel stub
$vmCode = @"
using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Utilities;
using ShoukatSons.POS.Wpf.ViewModels.Base;

namespace ShoukatSons.POS.Wpf.ViewModels
{
    public sealed class ${Name}ViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public ${Name}ViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            ExitCommand = new RelayCommand(() => _navigation.NavigateTo<DashboardViewModel>());
        }

        public string Header => "$Name";
        public string Info   => "Description here.";

        public ICommand ExitCommand { get; }
    }
}
"@

# Generate View stub (XAML)
$viewCode = @"
<UserControl x:Class=""ShoukatSons.POS.Wpf.Views.${Name}View""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Grid>
    <TextBlock Text=""{Binding Header}"" FontSize=""20"" Margin=""10""/>
    <TextBlock Text=""{Binding Info}"" Margin=""10,40,10,10""/>
    <Button Content=""Exit"" Command=""{Binding ExitCommand}"" HorizontalAlignment=""Right"" Margin=""10""/>
  </Grid>
</UserControl>
"@

# Write files
$vmPath   = Join-Path $vmDir   "$Name`ViewModel.cs"
$viewPath = Join-Path $viewDir "$Name`View.xaml"

Set-Content -Path $vmPath   -Value $vmCode   -Force
Set-Content -Path $viewPath -Value $viewCode -Force

Write-Host "Created ViewModel:" $vmPath
Write-Host "Created View:"      $viewPath