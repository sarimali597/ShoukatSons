<#
.SYNOPSIS
    Builds the full solution, migrates the database, and starts the WPF app.
.PARAMETER Clean
    Performs `dotnet clean` before building.
.PARAMETER Migrate
    Applies EF Core migrations after the build.
#>
param(
    [switch]$Clean,
    [switch]$Migrate
)

# 1. Locate repo root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Push-Location $scriptDir

# 2. Optionally clean
if ($Clean) {
    Write-Host "Cleaning solution…" -ForegroundColor Yellow
    dotnet clean
}

# 3. Build all projects
Write-Host "Building solution…" -ForegroundColor Yellow
dotnet build

# 4. Optionally run EF Core migrations
if ($Migrate) {
    Write-Host "Applying EF Core migrations…" -ForegroundColor Yellow
    dotnet ef database update `
        --project .\src\ShoukatSons.UI\ShoukatSons.UI.csproj `
        --startup-project .\src\ShoukatSons.UI\ShoukatSons.UI.csproj
}

# 5. Launch the WPF executable
Write-Host "Launching DashboardView…" -ForegroundColor Yellow
$exePath = Join-Path -Path $scriptDir `
    -ChildPath "src\ShoukatSons.UI\bin\Debug\net8.0-windows\ShoukatSons.UI.exe"

if (Test-Path $exePath) {
    Start-Process -FilePath $exePath
}
else {
    Write-Error "Cannot find WPF exe at $exePath. Build may have failed."
}

Pop-Location