# Initializes .NET solution, projects, references, and NuGet packages

$ErrorActionPreference = "Stop"
$root = "C:\ShoukatSons"
Set-Location $root

# 1) Create solution
if (-not (Test-Path "$root\ShoukatSons.sln" -PathType Leaf)) {
  dotnet new sln -n ShoukatSons | Out-Null
}

# 2) Create projects if missing
$projects = @(
  @{ Name="ShoukatSons.Core";     Template="classlib" ; Path="src\ShoukatSons.Core"     },
  @{ Name="ShoukatSons.Data";     Template="classlib" ; Path="src\ShoukatSons.Data"     },
  @{ Name="ShoukatSons.Services"; Template="classlib" ; Path="src\ShoukatSons.Services" },
  @{ Name="ShoukatSons.UI";       Template="wpf"      ; Path="src\ShoukatSons.UI"       },
  @{ Name="ShoukatSons.Tests";    Template="xunit"    ; Path="src\ShoukatSons.Tests"    }
)

foreach ($p in $projects) {
  $projDir = Join-Path $root $p.Path
  $csproj = Join-Path $projDir ($p.Name + ".csproj")
  if (-not (Test-Path $csproj)) {
    if (-not (Test-Path $projDir)) { New-Item -ItemType Directory -Path $projDir | Out-Null }
    dotnet new $p.Template -n $p.Name -o $projDir | Out-Null
  }
}

# 3) Add to solution
foreach ($p in $projects) {
  $csproj = Join-Path $root $p.Path ($p.Name + ".csproj")
  dotnet sln add $csproj 2>$null
}

# 4) Add project references
dotnet add src\ShoukatSons.Data\ShoukatSons.Data.csproj       reference src\ShoukatSons.Core\ShoukatSons.Core.csproj      2>$null
dotnet add src\ShoukatSons.Services\ShoukatSons.Services.csproj reference src\ShoukatSons.Core\ShoukatSons.Core.csproj    2>$null
dotnet add src\ShoukatSons.Services\ShoukatSons.Services.csproj reference src\ShoukatSons.Data\ShoukatSons.Data.csproj    2>$null
dotnet add src\ShoukatSons.UI\ShoukatSons.UI.csproj             reference src\ShoukatSons.Core\ShoukatSons.Core.csproj    2>$null
dotnet add src\ShoukatSons.UI\ShoukatSons.UI.csproj             reference src\ShoukatSons.Services\ShoukatSons.Services.csproj 2>$null
dotnet add src\ShoukatSons.Tests\ShoukatSons.Tests.csproj       reference src\ShoukatSons.Core\ShoukatSons.Core.csproj    2>$null
dotnet add src\ShoukatSons.Tests\ShoukatSons.Tests.csproj       reference src\ShoukatSons.Services\ShoukatSons.Services.csproj 2>$null

# 5) Add NuGet packages
dotnet add src\ShoukatSons.Data\ShoukatSons.Data.csproj package Microsoft.EntityFrameworkCore --version 8.0.6
dotnet add src\ShoukatSons.Data\ShoukatSons.Data.csproj package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.6

dotnet add src\ShoukatSons.Services\ShoukatSons.Services.csproj package System.Drawing.Common --version 8.0.6

dotnet add src\ShoukatSons.UI\ShoukatSons.UI.csproj package ZXing.Net --version 0.16.8
dotnet add src\ShoukatSons.UI\ShoukatSons.UI.csproj package CommunityToolkit.Mvvm --version 8.2.2

dotnet add src\ShoukatSons.Tests\ShoukatSons.Tests.csproj package FluentAssertions --version 6.12.0

# 6) Ensure UI is WPF trimmed correctly (disable trimming, set Windows)
$uiProj = Join-Path $root "src\ShoukatSons.UI\ShoukatSons.UI.csproj"
[xml]$xml = Get-Content $uiProj
$pg = $xml.Project.PropertyGroup | Select-Object -First 1
if (-not $pg.UseWPF) {
  $node = $xml.CreateElement("UseWPF"); $node.InnerText = "true"; $pg.AppendChild($node) | Out-Null
}
$rid = $xml.CreateElement("RuntimeIdentifier"); $rid.InnerText = "win10-x64"; $pg.AppendChild($rid) | Out-Null
$tfm = $xml.TargetFramework
$xml.Save($uiProj)

# 7) Restore and build once
dotnet restore
dotnet build -c Debug

Write-Host "✅ Solution initialized and built. Next: paste code files as provided." -ForegroundColor Green