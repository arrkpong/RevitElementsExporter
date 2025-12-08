# 🗺️ Development Roadmap

## แนวทางการพัฒนา RevitElementsExporter

---

## 📊 สถานะปัจจุบัน: v1.0.0

### ✅ Features ที่เสร็จแล้ว

| Feature         | สถานะ | Release |
| --------------- | ----- | ------- |
| Export CSV      | ✅    | v1.0.0  |
| Export JSON     | ✅    | v1.0.0  |
| Export Excel    | ✅    | v1.0.0  |
| Dark Mode       | ✅    | v1.0.0  |
| Category Filter | ✅    | v1.0.0  |
| Stats Preview   | ✅    | v1.0.0  |
| Progress Bar    | ✅    | v1.0.0  |
| Unit Tests      | ✅    | v1.0.0  |

---

## 🎯 Roadmap

### v1.1.0 - Enhanced Export (เป้าหมาย)

| Feature                          | Priority | ความซับซ้อน |
| -------------------------------- | -------- | ----------- |
| 🔲 Export Parameters ที่กำหนดเอง | สูง      | กลาง        |
| 🔲 Export เฉพาะ View ที่เลือก    | กลาง     | กลาง        |
| 🔲 Export BoundingBox            | กลาง     | ต่ำ         |
| 🔲 Export Room/Space data        | กลาง     | กลาง        |
| 🔲 Schedule export               | ต่ำ      | สูง         |

### v1.2.0 - Performance & UX

| Feature                        | Priority | ความซับซ้อน |
| ------------------------------ | -------- | ----------- |
| 🔲 Async export (ไม่ block UI) | สูง      | สูง         |
| 🔲 Save/Load filter presets    | กลาง     | กลาง        |
| 🔲 Remember last settings      | กลาง     | ต่ำ         |
| 🔲 Export history              | ต่ำ      | กลาง        |
| 🔲 Batch export multiple files | ต่ำ      | สูง         |

### v2.0.0 - Advanced Features

| Feature                         | Priority | ความซับซ้อน |
| ------------------------------- | -------- | ----------- |
| 🔲 Import data back to Revit    | ต่ำ      | สูง         |
| 🔲 Compare two models           | ต่ำ      | สูง         |
| 🔲 Auto-export on save          | ต่ำ      | กลาง        |
| 🔲 Cloud export (Google Sheets) | ต่ำ      | สูง         |

---

## 🏗️ Architecture Guidelines

### โครงสร้าง Folder

```
RevitElementsExporter/
├── Models/          # Data models (pure C#, no Revit deps)
├── Services/        # Business logic (testable)
├── ViewModels/      # MVVM ViewModels (future)
├── Views/           # WPF Views (XAML)
└── Themes/          # UI styles and colors
```

### หลักการออกแบบ

1. **Separation of Concerns**

   - Revit API logic อยู่ใน `ElementsExporter.cs` เท่านั้น
   - Export logic อยู่ใน `Services/`
   - UI logic อยู่ใน `ExportWindow.xaml.cs`

2. **Testability**

   - ทุก Service ต้อง test ได้โดยไม่ต้อง mock Revit API
   - ใช้ interface สำหรับ dependencies

3. **SOLID Principles**
   - Single Responsibility
   - Open/Closed
   - Dependency Inversion

---

## 🧪 Testing Guidelines

### Unit Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Naming Convention

```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange
    // Act
    // Assert
}
```

### What to Test

- ✅ ExportService methods
- ✅ CSV/JSON formatting
- ✅ Category filtering
- ✅ Stats calculation
- ❌ Revit API calls (mock or integration test)

---

## 📝 Coding Standards

### Naming Conventions

| Type           | Convention  | Example            |
| -------------- | ----------- | ------------------ |
| Class          | PascalCase  | `ElementExportRow` |
| Method         | PascalCase  | `ExportToCsv()`    |
| Property       | PascalCase  | `FilePath`         |
| Private field  | \_camelCase | `_isDarkMode`      |
| Local variable | camelCase   | `rowList`          |
| Constant       | PascalCase  | `FeetToMeters`     |

### Code Style

```csharp
// ✅ Good
public void ExportData(List<ElementExportRow> rows)
{
    if (rows == null || rows.Count == 0)
    {
        return;
    }

    foreach (var row in rows)
    {
        ProcessRow(row);
    }
}

// ❌ Bad
public void exportData(List<ElementExportRow> rows) {
    if (rows == null || rows.Count == 0) return;
    foreach (var row in rows) ProcessRow(row);
}
```

---

## 🔧 Development Setup

### Requirements

- Visual Studio 2022 or VS Code
- .NET 8.0 SDK
- Autodesk Revit 2026 (for testing)

### Build Commands

```powershell
# Restore packages
dotnet restore

# Build Debug
dotnet build

# Build Release
dotnet build -c Release

# Run tests
dotnet test

# Deploy to Revit
.\Deploy.ps1 -Configuration Release
```

---

## 🚀 Release Process

### 1. Update Version

- `RevitElementsExporter.csproj` - Version, AssemblyVersion
- `ExportWindow.xaml` - v1.x.x text

### 2. Build & Test

```powershell
dotnet build -c Release
dotnet test
```

### 3. Create Release

```powershell
# Create zip
dotnet publish -c Release -o publish
Compress-Archive -Path publish\* -DestinationPath "RevitElementsExporter-vX.X.X.zip"

# Create GitHub release
gh release create vX.X.X "RevitElementsExporter-vX.X.X.zip" --title "vX.X.X" --notes-file RELEASE_NOTES.md
```

---

## 🤝 Contributing

### Branch Strategy

```
master          # Production-ready code
└── develop     # Development branch
    ├── feature/xxx    # New features
    ├── bugfix/xxx     # Bug fixes
    └── hotfix/xxx     # Urgent fixes
```

### Pull Request Process

1. Fork repository
2. Create feature branch
3. Write code + tests
4. Submit PR to `develop`
5. Code review
6. Merge

### Commit Messages

```
feat: Add parameter export feature
fix: Fix CSV delimiter issue
docs: Update README
refactor: Extract ExportService
test: Add filtering tests
chore: Update dependencies
```

---

## 📚 Resources

### Revit API

- [Revit API Docs](https://www.revitapidocs.com/)
- [Building Coder](https://thebuildingcoder.typepad.com/)
- [Revit SDK Samples](https://www.autodesk.com/developer-network/platform-technologies/revit)

### .NET / WPF

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [WPF Tutorial](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)

---

## 📞 Contact

- **GitHub Issues**: Bug reports, feature requests
- **Discussions**: Questions, ideas

---

<div align="center">

**Happy Coding! 🚀**

</div>
