$ErrorActionPreference = "Stop"
Set-Location "C:\ShoukatSons"
dotnet restore
dotnet build -c Release
dotnet test -c Release
Write-Host "✅ Build + Tests succeeded (Release)" -ForegroundColor Green