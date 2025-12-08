# Repository Guidelines

## Project Structure & Module Organization

```
RevitXYZExporter/
├── RevitElementsExporter.sln          # Solution file
├── Deploy.ps1                         # Deployment script
├── README.md / README.th.md           # Documentation
│
├── RevitElementsExporter/             # Main add-in project
│   ├── RevitElementsExporter.csproj
│   ├── .addin                         # Revit manifest
│   ├── ElementsExporter.cs            # Main IExternalCommand
│   ├── ExportFormat.cs                # Export format enum
│   ├── ExportWindow.xaml/.cs          # WPF dialog
│   ├── Models/
│   │   └── ExportModels.cs            # Data models (ElementExportRow, CategoryInfo, ExportStats)
│   ├── Services/
│   │   └── ExportService.cs           # Pure export logic (testable, no Revit deps)
│   └── Themes/
│       ├── Colors.xaml                # Light mode palette
│       ├── ColorsDark.xaml            # Dark mode palette
│       └── Styles.xaml                # Control styles
│
└── RevitElementsExporter.Tests/       # Unit test project
    ├── RevitElementsExporter.Tests.csproj
    └── ExportServiceTests.cs          # Tests for ExportService
```

## Build, Test, and Development Commands

```powershell
# Build (Debug)
dotnet build

# Build (Release)
dotnet build -c Release

# Run tests
dotnet test

# Deploy to Revit
.\Deploy.ps1                           # Debug build
.\Deploy.ps1 -Configuration Release    # Release build
```

## Coding Style & Naming Conventions

- **Language**: C# 12, nullable enabled, implicit usings on
- **Indentation**: 4 spaces; braces on new lines
- **Naming**:
  - `PascalCase` for classes/methods/properties
  - `camelCase` for locals/parameters
  - `_camelCase` for private fields
- **CSV handling**: use `ExportService.EscapeCsv()` helper
- **Units**: Revit uses feet internally; convert to meters with `ExportService.FeetToMeters`

## Architecture

### Separation of Concerns

- **ElementsExporter.cs**: Revit API integration only - collects data from Document
- **ExportService.cs**: Pure C# logic - formatting, exporting, filtering (fully testable)
- **Models/**: Data transfer objects - no dependencies
- **ExportWindow**: WPF UI - handles user interaction

### Testability

- `ExportService` is static and has no Revit dependencies
- All helper methods are public for unit testing
- Tests link source files directly (no project reference to avoid Revit API)

## Testing Guidelines

- Test project: `RevitElementsExporter.Tests` (xUnit)
- Target framework: `net8.0-windows`
- Tests cover:
  - `FormatNullable()` - null handling
  - `EscapeCsv()` - comma and quote escaping
  - `CalculateStats()` - statistics calculation
  - `FilterByCategories()` - category filtering
  - `GetCategorySummary()` - category grouping

Run tests:

```powershell
dotnet test
```

## Commit & Pull Request Guidelines

- **Commits**: concise, imperative (e.g., "Add category filter to export dialog")
- **PRs**: describe what/why/how, include test results
- **Breaking changes**: document in PR description

## Build/Runtime Environment

- **Framework**: `net8.0-windows` (x64)
- **Revit version**: 2026
- **References**: `RevitAPI.dll`, `RevitAPIUI.dll` from `C:\Program Files\Autodesk\Revit 2026\`
- **Dependencies**: `DocumentFormat.OpenXml` 3.1.0

## Deployment

Use the deploy script:

```powershell
.\Deploy.ps1 -Configuration Release -RevitVersion 2026
```

Or manually copy to: `%AppData%\Autodesk\Revit\Addins\2026\`

Files needed:

- `RevitElementsExporter.dll`
- `DocumentFormat.OpenXml.dll`
- `DocumentFormat.OpenXml.Framework.dll`
- `RevitElementsExporter.addin`
