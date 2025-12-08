# RevitElementsExporter Deploy Script
# Run this script to deploy the add-in to Revit

param(
    [string]$RevitVersion = "2026",
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RevitElementsExporter Deploy Script  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Revit Version: $RevitVersion" -ForegroundColor Yellow
Write-Host ""

# Paths
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$SourceDir = Join-Path $ScriptDir "RevitElementsExporter\bin\$Configuration\net8.0-windows"
$TargetDir = "$env:APPDATA\Autodesk\Revit\Addins\$RevitVersion"

# Check if build exists
$DllPath = Join-Path $SourceDir "RevitElementsExporter.dll"
if (-not (Test-Path $DllPath)) {
    Write-Host "[ERROR] DLL not found at: $DllPath" -ForegroundColor Red
    Write-Host "Please run 'dotnet build -c $Configuration' first." -ForegroundColor Yellow
    exit 1
}

# Create target directory if it doesn't exist
if (-not (Test-Path $TargetDir)) {
    Write-Host "[INFO] Creating directory: $TargetDir" -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $TargetDir -Force | Out-Null
}

# Copy files
Write-Host "[1/3] Copying DLL ($Configuration)..." -ForegroundColor White
Copy-Item $DllPath $TargetDir -Force
Write-Host "      -> RevitElementsExporter.dll" -ForegroundColor Green

# Copy dependencies
Write-Host "[2/3] Copying dependencies..." -ForegroundColor White
$Dependencies = @(
    "DocumentFormat.OpenXml.dll",
    "DocumentFormat.OpenXml.Framework.dll"
)
foreach ($dep in $Dependencies) {
    $depPath = Join-Path $SourceDir $dep
    if (Test-Path $depPath) {
        Copy-Item $depPath $TargetDir -Force
        Write-Host "      -> $dep" -ForegroundColor Green
    }
}

# Create .addin file with correct path
Write-Host "[3/3] Creating .addin manifest..." -ForegroundColor White
$AddinContent = @"
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
	<AddIn Type="Command">
		<Name>Revit Elements Exporter</Name>
		<Assembly>$TargetDir\RevitElementsExporter.dll</Assembly>
		<AddInId>1A78401D-2894-4F29-B49D-F847C362EFAA</AddInId>
		<FullClassName>RevitElementsExporter.ElementsExporter</FullClassName>
		<Text>Export Elements</Text>
		<Description>Export all element coordinates and metadata to CSV, JSON, or Excel</Description>
		<VendorId>MyCompany</VendorId>
		<VendorDescription>Revit Elements Exporter Add-in</VendorDescription>
		<VisibilityMode>AlwaysVisible</VisibilityMode>
	</AddIn>
</RevitAddIns>
"@
$AddinPath = Join-Path $TargetDir "RevitElementsExporter.addin"
$AddinContent | Out-File -FilePath $AddinPath -Encoding UTF8
Write-Host "      -> RevitElementsExporter.addin" -ForegroundColor Green

# Show file info
Write-Host ""
Write-Host "Files deployed:" -ForegroundColor Cyan
Get-ChildItem $TargetDir -Filter "RevitElements*" | ForEach-Object {
    $size = [math]::Round($_.Length / 1KB, 2)
    Write-Host "  $($_.Name) ($size KB)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Deploy completed successfully!       " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Deployed to: $TargetDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Start/Restart Revit $RevitVersion" -ForegroundColor White
Write-Host "  2. Go to Add-Ins -> External Tools -> Export Elements" -ForegroundColor White
Write-Host ""
