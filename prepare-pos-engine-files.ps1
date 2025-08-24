param(
  [string]$Root = (Get-Location).Path,
  [switch]$Overwrite,
  [switch]$Preview
)

$ErrorActionPreference = "Stop"

# Required files for POS Core Engine
$files = @(
  "src\ShoukatSons.Core\POS\Entities.cs",
  "src\ShoukatSons.Core\POS\DTOs.cs",
  "src\ShoukatSons.Core\POS\Services.cs",
  "src\ShoukatSons.Data\POSDbContext.cs",
  "src\ShoukatSons.Services\Implementations\POS\TransactionService.cs",
  "src\ShoukatSons.Services\Implementations\POS\InventoryService.cs",
  "src\ShoukatSons.Services\Implementations\POS\ReportingService.cs",
  "src\ShoukatSons.Services\Interfaces\POS\ITransactionService.cs",
  "src\ShoukatSons.Services\Interfaces\POS\IInventoryService.cs",
  "src\ShoukatSons.Services\Interfaces\POS\IReportingService.cs"
)

function New-Header($rel) {
@"
// Auto-created by prepare-pos-engine-files.ps1
// Path: $rel
// Paste the provided content for this file here.
"@
}

# Backup root if overwriting
$backupRoot = Join-Path $Root ("_backup_" + (Get-Date -Format 'yyyyMMdd_HHmmss'))

foreach ($rel in $files) {
    $full = Join-Path $Root $rel
    $dir  = Split-Path $full

    if ($Preview) {
        if (!(Test-Path $dir)) { Write-Host "[CreateDir] $dir" -ForegroundColor Cyan }
        if (Test-Path $full) {
            if ($Overwrite) {
                Write-Host "[Backup+Recreate] $full -> $backupRoot" -ForegroundColor Yellow
            } else {
                Write-Host "[SkipExists] $full" -ForegroundColor DarkYellow
            }
        } else {
            Write-Host "[CreateFile] $full" -ForegroundColor Green
        }
        continue
    }

    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Force -Path $dir | Out-Null
        Write-Host "Created directory: $dir"
    }

    if (Test-Path $full) {
        if ($Overwrite) {
            $backupPath = Join-Path $backupRoot $rel
            $backupDir  = Split-Path $backupPath
            if (!(Test-Path $backupDir)) { New-Item -ItemType Directory -Force -Path $backupDir | Out-Null }
            Move-Item -Path $full -Destination $backupPath -Force
            Set-Content -Path $full -Encoding UTF8 -Value (New-Header $rel)
            Write-Host "Backed up and recreated: $rel"
        } else {
            Write-Host "Exists (kept): $rel"
        }
    } else {
        Set-Content -Path $full -Encoding UTF8 -Value (New-Header $rel)
        Write-Host "Created file: $rel"
    }
}

Write-Host "`nDone."
if ($Preview) { Write-Host "Preview only. No changes made." -ForegroundColor DarkGray }