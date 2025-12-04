# RevitElementsExporter

Revit add-in that exports all element coordinates to a CSV on the user's Desktop. It handles points and curves, converts Revit internal feet to meters, and writes metadata such as category, family, type, and level.

## Project Structure
- `RevitElementsExporter.sln` - Visual Studio solution.
- `RevitElementsExporter/` - add-in source (`ExportCoordinates.cs`), project file (`RevitElementsExporter.csproj`), and add-in manifest (`.addin`).
- `bin/`, `obj/` - build outputs (generated).

## Prerequisites
- .NET 8 SDK.
- Autodesk Revit 2026 (paths in the project reference `RevitAPI.dll` and `RevitAPIUI.dll` from `C:\Program Files\Autodesk\Revit 2026\`).
- Windows x64 (project targets `net8.0-windows`, `PlatformTarget` x64).

## Build
Run from the repository root:

```powershell
dotnet build
```

This restores packages and compiles the add-in DLL to `RevitElementsExporter/bin/Debug/net8.0-windows/`.

## Install / Deploy
1) Copy `RevitElementsExporter/bin/Debug/net8.0-windows/RevitElementsExporter.dll` to your Revit add-ins folder, typically `%AppData%\Autodesk\Revit\Addins\2026\`.
2) Copy `RevitElementsExporter/.addin` to the same folder and update the `<Assembly>` path in the manifest if your build output path differs.
3) Restart Revit.

## Usage
Open Revit -> Add-Ins tab -> External Tools -> Revit Elements Exporter. The command writes `RevitAllElements.csv` to the current user's Desktop with columns for Id, category, family, type, level, location type, and coordinates (meters). Curves include start/end XYZ columns.

## Development Notes
- API references are marked `Private=false` to rely on Revit-installed assemblies; ensure the hint paths match your Revit version if you upgrade/downgrade.
- Keep coordinate conversions centralized via the `FeetToMeters` constant in `ExportCoordinates.cs`.
- If you add tests, create a sibling test project and mock Revit API calls instead of loading the real DLLs.
