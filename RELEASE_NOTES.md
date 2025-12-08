# 🎉 Revit Elements Exporter v1.0.0

First official release of Revit Elements Exporter!

## ✨ Features

- **Multi-format Export** - CSV, JSON, and Excel (.xlsx)
- **Coordinate Conversion** - Automatic feet to meters conversion
- **Modern UI** - Beautiful WPF dialog with Light/Dark mode
- **Category Filter** - Filter elements by category before export
- **Stats Preview** - View element counts before exporting
- **Progress Bar** - Track export progress
- **Resizable Window** - Adjust window size as needed

## 📊 Export Data

Each element exports with:

- Id, Category, Family, Type, Level
- LocationType (Point/Curve/None)
- X/Y/Z coordinates (in meters)
- StartX/Y/Z, EndX/Y/Z for curves

## 🔧 Requirements

- Autodesk Revit 2026
- .NET 8.0

## 📥 Installation

1. Download `RevitElementsExporter-v1.0.0.zip`
2. Extract all files to `%AppData%\Autodesk\Revit\Addins\2026\`
3. Restart Revit
4. Find in: **Add-Ins → External Tools → Export Elements**

## 📦 What's Included

- `RevitElementsExporter.dll` - Main add-in
- `DocumentFormat.OpenXml.dll` - Excel support
- `DocumentFormat.OpenXml.Framework.dll` - Excel support
- `RevitElementsExporter.addin` - Revit manifest

## 🌙 Dark Mode

Click the moon icon (🌙) in the header to switch to Dark Mode!
