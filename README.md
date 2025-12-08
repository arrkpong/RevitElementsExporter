# RevitElementsExporter

<div align="center">

![Revit](https://img.shields.io/badge/Revit-2026-blue?style=for-the-badge&logo=autodesk)
![.NET](https://img.shields.io/badge/.NET-8.0-purple?style=for-the-badge&logo=dotnet)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**Export all Revit elements with coordinates and metadata to CSV, JSON, or Excel**

</div>

---

## ✨ Features

- 📊 **Multi-format Export** - CSV, JSON, and Excel (.xlsx via OpenXML)
- 🎯 **Coordinate Conversion** - Automatic feet to meters conversion
- 🎨 **Modern UI** - Beautiful WPF dialog with Light/Dark mode support
- 📁 **Category Filter** - Filter elements by category before export
- 📈 **Progress Tracking** - Real-time progress bar during export
- 🔍 **Element Preview** - Preview element count before exporting

## 📋 Output Data

Each element exports with:
| Column | Description |
|--------|-------------|
| `Id` | Element ID |
| `Category` | Category name |
| `Family` | Family name (for FamilyInstance) |
| `Type` | Type name |
| `Level` | Associated level |
| `LocationType` | Point, Curve, Curve-Unbound, or None |
| `X, Y, Z` | Point coordinates (meters) |
| `StartX/Y/Z, EndX/Y/Z` | Curve endpoints (meters) |

## 📁 Project Structure

```
RevitElementsExporter/
├── .addin                    # Revit add-in manifest
├── ElementsExporter.cs       # Main command class
├── ExportFormat.cs           # Format enum
├── ExportWindow.xaml         # WPF dialog UI
├── ExportWindow.xaml.cs      # Dialog logic
├── RevitElementsExporter.csproj
└── Themes/
    ├── Colors.xaml           # Color palette (Light/Dark)
    └── Styles.xaml           # Control styles
```

## 🔧 Requirements

- **.NET 8 SDK** (Windows x64)
- **Autodesk Revit 2026**
- References: `RevitAPI.dll`, `RevitAPIUI.dll` from `C:\Program Files\Autodesk\Revit 2026\`

## 🚀 Build

```powershell
cd RevitXYZExporter
dotnet build
```

Output: `RevitElementsExporter/bin/Debug/net8.0-windows/RevitElementsExporter.dll`

## 📦 Install / Deploy

### Option 1: Manual

1. Copy `RevitElementsExporter.dll` to `%AppData%\Autodesk\Revit\Addins\2026\`
2. Copy `.addin` manifest to the same folder
3. Update `<Assembly>` path in `.addin` if needed
4. Restart Revit

### Option 2: Script (PowerShell)

```powershell
# Run from project root
$source = "RevitElementsExporter\bin\Debug\net8.0-windows"
$target = "$env:APPDATA\Autodesk\Revit\Addins\2026"
Copy-Item "$source\RevitElementsExporter.dll" $target -Force
Copy-Item "RevitElementsExporter\.addin" $target -Force
```

## 🎮 Usage

1. Open Revit → **Add-Ins** → **External Tools** → **Export Elements**
2. Choose export format (CSV / JSON / Excel)
3. Set destination file path
4. Click **Export** ✓

## 🎨 UI Features

- **Modern Design** - Gradient headers, smooth animations, shadow effects
- **Light/Dark Mode** - Toggle between themes
- **Responsive Layout** - Clean organization with icons

## 🛠 Development Notes

- Coordinate conversion: `FeetToMeters = 0.3048`
- Excel export uses `DocumentFormat.OpenXml` 3.1.0 (no Excel interop needed)
- Revit API references set to `Private=false` to use installed binaries
- Main class: `ElementsExporter` implements `IExternalCommand`

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.
