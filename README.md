# RevitElementsExporter

Revit add-in (net8.0-windows) that exports all elements with coordinates and basic metadata. A built-in WPF dialog lets you pick output format (CSV, JSON, Excel) and the destination file path. Coordinates are converted from Revit internal feet to meters.

## Features
- Exports instances (non-types) with: Id, Category, Family, Type, Level, LocationType, XYZ, and Start/End XYZ for curves.
- Supports three formats: CSV, JSON, Excel (.xlsx via OpenXML).
- Pure WPF dialog to choose format and file path; default path is `RevitAllElements.csv` on Desktop.
- Handles unbounded curves by labeling `LocationType` as `Curve-Unbound`.

## Project Structure
- `RevitElementsExporter.sln` – solution file.
- `RevitElementsExporter/` – add-in source (`ExportCoordinates.cs`, `ExportWindow.xaml`), project (`RevitElementsExporter.csproj`), theme resources (`Themes/`), and add-in manifest (`.addin`).
- `bin/`, `obj/` – build outputs (generated).

## Requirements
- .NET 8 SDK.
- Windows x64.
- Autodesk Revit 2026; project references `RevitAPI.dll` and `RevitAPIUI.dll` from `C:\Program Files\Autodesk\Revit 2026\`.

## Build
From repo root:
```powershell
dotnet build
```
Output DLL is at `RevitElementsExporter/bin/Debug/net8.0-windows/RevitElementsExporter.dll`.

## Install / Deploy
1. Copy `RevitElementsExporter/bin/Debug/net8.0-windows/RevitElementsExporter.dll` to `%AppData%\Autodesk\Revit\Addins\2026\` (or your target version folder).
2. Copy `RevitElementsExporter/.addin` to the same folder and update the `<Assembly>` path if your build path differs.
3. Restart Revit.

## Usage
1. In Revit: Add-Ins → External Tools → Revit Elements Exporter.
2. Dialog options:
   - **Format**: CSV / JSON / Excel (.xlsx).
   - **File path**: default on Desktop; use Browse to change.
3. Click **Export**. The file is written with coordinates in meters.

## Output Columns
`Id, Category, Family, Type, Level, LocationType, X, Y, Z, StartX, StartY, StartZ, EndX, EndY, EndZ`
- `X/Y/Z`: point location (meters) when `LocationType=Point`.
- `Start*/End*`: curve endpoints (meters) when `LocationType=Curve`; unbounded curves get `Curve-Unbound`.

## Development Notes
- Coordinate conversion uses the `FeetToMeters` constant in `ExportCoordinates.cs`.
- UI lives in `ExportWindow.xaml` with shared styles under `Themes/`.
- Excel export uses `DocumentFormat.OpenXml` (3.1.0); no interop dependency.
- Revit API references are `Private=false` to rely on installed binaries; adjust hint paths if your Revit version changes.
- If adding tests, create a sibling test project and mock Revit API calls (do not load real Revit assemblies in tests).
