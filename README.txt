Shoukat Sons Retail POS - Quick Start
=====================================

Requirements
- Windows 10+
- .NET 6 Desktop Runtime
- NuGet: ZXing.Net, System.Configuration.ConfigurationManager, System.Drawing.Common (Windows)

Build & Publish
1) dotnet restore
2) dotnet build -c Release
3) dotnet publish .\src\ShoukatSons.UI\ -c Release -r win-x64 --self-contained false

Config
- src\ShoukatSons.UI\App.config
  - BackupDirectory = C:\ProgramData\ShoukatSons\Backups
  - CloudSyncEndpoint = https://your-cloud-endpoint/api/upload

Deploy
- PowerShell Admin → .\installer\InstallerScript.ps1

Printing Test
- Run UI → set printer + label options → Save → Print Sample

Troubleshooting
- Logs: %ProgramData%\ShoukatSons\Logs\error.log
- Ensure printer driver installed and app outbound HTTP allowed