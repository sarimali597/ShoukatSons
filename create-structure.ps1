# === Shoukat Sons Retail POS Project Structure Creator ===
# Is script ko .ps1 file me save karke PowerShell me run karo

$root = "C:\ShoukatSons"

# Folder structure as array of paths (relative to $root)
$folders = @(
    "src\ShoukatSons.Core\Models",
    "src\ShoukatSons.Core\Interfaces",
    "src\ShoukatSons.Core\Helpers",
    "src\ShoukatSons.Data\Repositories",
    "src\ShoukatSons.Services",
    "src\ShoukatSons.UI\Assets",
    "src\ShoukatSons.UI\Views",
    "src\ShoukatSons.UI\ViewModels",
    "src\ShoukatSons.UI\Services\Printing",
    "src\ShoukatSons.Tests",
    "installer"
)

# Files with relative path from $root
$files = @(
    "ShoukatSons.sln",
    "build.ps1",
    "publish.ps1",
    "src\ShoukatSons.Core\Models\Product.cs",
    "src\ShoukatSons.Core\Models\Category.cs",
    "src\ShoukatSons.Core\Models\Vendor.cs",
    "src\ShoukatSons.Core\Models\Sale.cs",
    "src\ShoukatSons.Core\Models\SaleItem.cs",
    "src\ShoukatSons.Core\Models\User.cs",
    "src\ShoukatSons.Core\Models\LabelSettings.cs",
    "src\ShoukatSons.Core\Interfaces\IRepository.cs",
    "src\ShoukatSons.Core\Interfaces\IBarcodeService.cs",
    "src\ShoukatSons.Core\Interfaces\IBackupService.cs",
    "src\ShoukatSons.Core\Interfaces\IAuthService.cs",
    "src\ShoukatSons.Core\Helpers\PasswordHasher.cs",
    "src\ShoukatSons.Core\Helpers\RelayCommand.cs",
    "src\ShoukatSons.Core\Helpers\AsyncCommand.cs",
    "src\ShoukatSons.Data\DatabaseContext.cs",
    "src\ShoukatSons.Data\Repositories\GenericRepository.cs",
    "src\ShoukatSons.Data\Repositories\ProductRepository.cs",
    "src\ShoukatSons.Data\Repositories\SaleRepository.cs",
    "src\ShoukatSons.Services\AuthService.cs",
    "src\ShoukatSons.Services\BarcodeService.cs",
    "src\ShoukatSons.Services\BackupService.cs",
    "src\ShoukatSons.Services\PrintService.cs",
    "src\ShoukatSons.Services\ReportService.cs",
    "src\ShoukatSons.UI\App.xaml",
    "src\ShoukatSons.UI\App.xaml.cs",
    "src\ShoukatSons.UI\Views\LoginView.xaml",
    "src\ShoukatSons.UI\Views\DashboardView.xaml",
    "src\ShoukatSons.UI\Views\ProductsView.xaml",
    "src\ShoukatSons.UI\Views\POSView.xaml",
    "src\ShoukatSons.UI\Views\ReportsView.xaml",
    "src\ShoukatSons.UI\Views\SettingsView.xaml",
    "src\ShoukatSons.UI\Views\BarcodePrintView.xaml",
    "src\ShoukatSons.UI\ViewModels\LoginViewModel.cs",
    "src\ShoukatSons.UI\ViewModels\DashboardViewModel.cs",
    "src\ShoukatSons.UI\ViewModels\ProductsViewModel.cs",
    "src\ShoukatSons.UI\ViewModels\POSViewModel.cs",
    "src\ShoukatSons.UI\ViewModels\ReportsViewModel.cs",
    "src\ShoukatSons.UI\ViewModels\SettingsViewModel.cs",
    "src\ShoukatSons.UI\ViewModels\BarcodePrintViewModel.cs",
    "src\ShoukatSons.UI\Services\NavigationService.cs",
    "src\ShoukatSons.UI\Services\DialogService.cs",
    "src\ShoukatSons.UI\Services\Printing\LabelComposer.cs",
    "src\ShoukatSons.UI\Services\Printing\BarcodeGenerator.cs",
    "src\ShoukatSons.UI\Services\Printing\PrinterResolver.cs",
    "src\ShoukatSons.Tests\CoreTests.cs",
    "src\ShoukatSons.Tests\ServiceTests.cs",
    "src\ShoukatSons.Tests\UITests.cs",
    "installer\config.wxs",
    "installer\postinstall-guide.pdf"
)

# Create root folder
if (-not (Test-Path $root)) {
    New-Item -ItemType Directory -Path $root | Out-Null
}

# Create subfolders
foreach ($folder in $folders) {
    $fullPath = Join-Path $root $folder
    if (-not (Test-Path $fullPath)) {
        New-Item -ItemType Directory -Path $fullPath | Out-Null
    }
}

# Create empty files
foreach ($file in $files) {
    $fullPath = Join-Path $root $file
    if (-not (Test-Path $fullPath)) {
        New-Item -ItemType File -Path $fullPath | Out-Null
    }
}

Write-Host "✅ Folder structure and placeholder files created successfully at $root" -ForegroundColor Green