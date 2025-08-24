param(
    [string]$UiProjectPublishDir = "$(Resolve-Path ..\src\ShoukatSons.UI\bin\Release\net6.0-windows\publish)",
    [string]$TargetDir = "C:\Program Files\Shoukat Sons Retail POS"
)

Write-Host "Publishing from: $UiProjectPublishDir"

if (-not (Test-Path $UiProjectPublishDir)) {
    Write-Error "Publish directory not found. Run: dotnet publish .\src\ShoukatSons.UI\ -c Release -r win-x64 --self-contained false"
    exit 1
}

if (Test-Path $TargetDir) {
    $backup = "$TargetDir.backup.$((Get-Date).ToString('yyyyMMddHHmmss'))"
    Write-Host "Existing install found. Backing up to: $backup"
    Rename-Item -Path $TargetDir -NewName $backup -ErrorAction SilentlyContinue
}

New-Item -Path $TargetDir -ItemType Directory -Force | Out-Null
Copy-Item -Path (Join-Path $UiProjectPublishDir "*") -Destination $TargetDir -Recurse -Force

Write-Host "✅ Deployment completed to $TargetDir"