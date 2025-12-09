# Build and Deploy Script for Revit Elements Exporter
# Combines auto-build convenience with robust deployment logic

param(
    [string]$RevitVersion = "2026",
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

# === 1. Setup Paths ===
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$SolutionDir = Resolve-Path "$ScriptDir\.."
$ProjectDir = "$SolutionDir\RevitElementsExporter"
$BuildOutput = "$ProjectDir\bin\$Configuration\net8.0-windows"

# Revit Addins Folder
$AddinFolder = "$env:APPDATA\Autodesk\Revit\Addins\$RevitVersion"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RevitElementsExporter - Build & Deploy" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Revit Version: $RevitVersion" -ForegroundColor Yellow
Write-Host ""

# === 2. Build Solution ===
Write-Host "[1/5] Building Solution..." -ForegroundColor White
dotnet build "$SolutionDir\RevitElementsExporter.sln" --configuration $Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build Failed. Aborting deployment."
}

# === 3. Run Tests ===
Write-Host "[2/5] Running Tests..." -ForegroundColor White
dotnet test "$SolutionDir\RevitElementsExporter.sln" --configuration $Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests Failed. Aborting deployment."
}

# === 4. Create Target Directory ===
if (-not (Test-Path $AddinFolder)) {
    Write-Host "Creating Addins folder: $AddinFolder" -ForegroundColor Yellow
    New-Item -ItemType Directory -Force -Path $AddinFolder | Out-Null
}

# === 5. Copy Files ===
Write-Host "[3/5] Deploying DLL..." -ForegroundColor White

# Copy DLL
$DllPath = "$BuildOutput\RevitElementsExporter.dll"
if (Test-Path $DllPath) {
    Copy-Item $DllPath $AddinFolder -Force
    Write-Host "      -> RevitElementsExporter.dll" -ForegroundColor Green
}
else {
    Write-Error "Build output not found at $DllPath"
}

# Copy Dependencies
Write-Host "[4/5] Copying dependencies..." -ForegroundColor White
$Dependencies = @(
    "DocumentFormat.OpenXml.dll",
    "DocumentFormat.OpenXml.Framework.dll"
)
foreach ($dep in $Dependencies) {
    $depPath = "$BuildOutput\$dep"
    if (Test-Path $depPath) {
        Copy-Item $depPath $AddinFolder -Force
        Write-Host "      -> $dep" -ForegroundColor Green
    }
}

# === 6. Generate Manifest (.addin) ===
Write-Host "[5/5] Generating Manifest..." -ForegroundColor White

# We point to the deployed DLL in the Addins folder
$AssemblyPath = "$AddinFolder\RevitElementsExporter.dll"

$AddinContent = @"
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
	<AddIn Type="Command">
		<Name>Revit Elements Exporter</Name>
		<Assembly>$AssemblyPath</Assembly>
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

$AddinFilePath = "$AddinFolder\RevitElementsExporter.addin"
$AddinContent | Out-File -FilePath $AddinFilePath -Encoding UTF8
Write-Host "      -> RevitElementsExporter.addin" -ForegroundColor Green

# === 6. Summary ===
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Success! Deployed to Revit $RevitVersion  " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Deployed to: $AddinFolder" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Start/Restart Revit $RevitVersion" -ForegroundColor White
Write-Host "  2. Go to Add-Ins -> External Tools -> Export Elements" -ForegroundColor White
Write-Host ""
