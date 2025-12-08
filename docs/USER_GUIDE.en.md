# 📖 User Guide - Revit Elements Exporter

<div align="center">

![Version](https://img.shields.io/badge/Version-1.0.0-blue?style=flat-square)
![Revit](https://img.shields.io/badge/Revit-2026-orange?style=flat-square)

**Export Revit Elements with Coordinates and Metadata**

</div>

---

## 📋 Table of Contents

1. [Installation](#-installation)
2. [Getting Started](#-getting-started)
3. [Main Interface](#-main-interface)
4. [Export Settings](#-export-settings)
5. [Category Filter](#-category-filter)
6. [File Formats](#-file-formats)
7. [Exported Data](#-exported-data)
8. [Dark Mode](#-dark-mode)
9. [Troubleshooting](#-troubleshooting)

---

## 🔧 Installation

### Method 1: Automatic Script (Recommended)

```powershell
# Open PowerShell and run
.\Deploy.ps1 -Configuration Release
```

### Method 2: Manual Installation

1. Copy the following files to `%AppData%\Autodesk\Revit\Addins\2026\`:

   - `RevitElementsExporter.dll`
   - `DocumentFormat.OpenXml.dll`
   - `DocumentFormat.OpenXml.Framework.dll`
   - `RevitElementsExporter.addin`

2. Restart Revit

### Verify Installation

After installation and opening Revit, you should see:

- **Add-Ins** → **External Tools** → **Export Elements**

---

## 🚀 Getting Started

1. Open a Revit project file
2. Go to **Add-Ins** menu
3. Click **External Tools**
4. Select **Export Elements**

---

## 🖥 Main Interface

### Interface Layout

```
┌─────────────────────────────────────────────┐
│  🏠 Header - Title and Theme Toggle         │
├─────────────────────────────────────────────┤
│  📊 Stats Card - Element statistics         │
│  ┌─────┬─────┬─────┬─────┐                 │
│  │Total│Point│Curve│ Cat │                 │
│  └─────┴─────┴─────┴─────┘                 │
├─────────────────────────────────────────────┤
│  ┌───────────────────┬──────────────┐       │
│  │ Format & Path     │ Category     │       │
│  │ • File format     │ Filter       │       │
│  │ • File location   │ • Walls      │       │
│  │                   │ • Doors      │       │
│  │ Info Box          │ • Windows    │       │
│  └───────────────────┴──────────────┘       │
├─────────────────────────────────────────────┤
│  v1.0.0              [Cancel] [✓ Export]   │
└─────────────────────────────────────────────┘
```

---

## ⚙ Export Settings

### 1. Select File Format

Click the "File Format" dropdown and choose:

| Format   | Description                | Best For          |
| -------- | -------------------------- | ----------------- |
| 📄 CSV   | Comma-Separated Values     | Excel, Databases  |
| 📋 JSON  | JavaScript Object Notation | Web Apps, APIs    |
| 📊 Excel | .xlsx file                 | Reports, Analysis |

### 2. Set File Location

- Default: `Desktop\RevitAllElements.csv`
- Click 📁 button to choose a different location
- File extension changes automatically based on format

---

## 🔍 Category Filter

### Using the Category Filter

The right side of the window shows categories with element counts:

```
☑ Walls (1,234)
☑ Doors (567)
☑ Windows (890)
☐ Levels (10)
☐ Views (45)
```

### Quick Select Buttons

| Button          | Action                  |
| --------------- | ----------------------- |
| **Select All**  | Select all categories   |
| **Select None** | Deselect all categories |

### Real-time Stats Update

When selecting/deselecting categories, the stats update immediately:

- **Total**: Number of elements to export
- **Point**: Elements with point location
- **Curve**: Elements with curve location
- **Categories**: Number of selected categories

---

## 📁 File Formats

### CSV Format

```csv
Id,Category,Family,Type,Level,LocationType,X,Y,Z,StartX,StartY,StartZ,EndX,EndY,EndZ
123456,Walls,Basic Wall,Generic 200mm,Level 1,Curve,,,,-5.2345,10.1234,0.0000,8.7654,10.1234,0.0000
```

### JSON Format

```json
[
  {
    "Id": "123456",
    "Category": "Walls",
    "Family": "Basic Wall",
    "Type": "Generic 200mm",
    "Level": "Level 1",
    "LocationType": "Curve",
    "StartX": -5.2345,
    "StartY": 10.1234,
    ...
  }
]
```

### Excel Format

Ready-to-use .xlsx file with headers in the first row.

---

## 📊 Exported Data

### All Columns

| Column         | Type   | Description                          |
| -------------- | ------ | ------------------------------------ |
| `Id`           | Text   | Element ID                           |
| `Category`     | Text   | Category name (e.g., Walls, Doors)   |
| `Family`       | Text   | Family name (for FamilyInstance)     |
| `Type`         | Text   | Type name                            |
| `Level`        | Text   | Associated level                     |
| `LocationType` | Text   | Point / Curve / Curve-Unbound / None |
| `X, Y, Z`      | Number | Point coordinates (meters)           |
| `StartX/Y/Z`   | Number | Curve start point (meters)           |
| `EndX/Y/Z`     | Number | Curve end point (meters)             |

### Coordinate Units

> ⚠️ **Important**: All coordinates are automatically converted to **meters** (from Revit's internal feet)

### Location Types

| LocationType    | Description           | Example Elements          |
| --------------- | --------------------- | ------------------------- |
| `Point`         | Single point location | Doors, Windows, Furniture |
| `Curve`         | Start and end points  | Walls, Lines, Beams       |
| `Curve-Unbound` | Unbounded line        | Reference lines           |
| `None`          | No physical location  | Views, Levels, Types      |

---

## 🌙 Dark Mode

### How to Enable

Click the 🌙 button in the top-right corner of the header.

| Mode  | Icon | Best For                    |
| ----- | ---- | --------------------------- |
| Light | 🌙   | General use                 |
| Dark  | ☀️   | Reduced eye strain at night |

---

## ❓ Troubleshooting

### Export Elements menu not visible

1. Check that `.addin` file is in the correct folder
2. Verify the path in `.addin` matches the DLL location
3. Restart Revit

### Export fails / Error

1. Make sure a project is open
2. Check that the destination path is writable
3. Try changing the file location to Desktop

### File has no data

- Check that some categories are selected
- Look at the Total count at the top - if it's 0, no elements match

### Coordinates seem wrong

- Coordinates are in **meters** (not feet)
- Check the Project Base Point in Revit for coordinate system

---

## 📞 Contact

If you encounter issues or want new features:

- Open an Issue on the GitHub repository
- Or contact the developer directly

---

<div align="center">

**Made with ❤️ for Revit Users**

</div>
