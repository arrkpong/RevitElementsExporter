# 🎉 Revit Elements Exporter v1.0.0

First official release of Revit Elements Exporter!

## ✨ Features

- **Multi-format Export** - CSV, JSON, and Excel (.xlsx)
- **Coordinate Conversion** - Automatic feet to meters conversion
- **Modern UI** - Beautiful WPF dialog with Light/Dark mode
- **Category Filter** - Filter elements by category before export
- **Stats Preview** - View element counts before exporting
- **Progress Bar** - Track export progress

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

1. Download the release files
2. Extract to `%AppData%\Autodesk\Revit\Addins\2026\`
3. Restart Revit
4. Find in: **Add-Ins → External Tools → Export Elements**

## 📝 What's New

- Modern UI with gradient headers and icons
- Dark mode support
- Category filter for selective export
- Real-time element stats preview
- Resizable window
- Unit tests included
