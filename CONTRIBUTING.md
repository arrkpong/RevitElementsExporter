# 🤝 Contributing Guide

ยินดีต้อนรับการมีส่วนร่วมในการพัฒนา RevitElementsExporter!

---

## 📋 วิธีการมีส่วนร่วม

### 1. รายงาน Bug 🐛

เมื่อพบ bug กรุณาเปิด Issue พร้อมข้อมูล:

- **สิ่งที่เกิดขึ้น**: อธิบายปัญหาที่พบ
- **สิ่งที่คาดหวัง**: ควรทำงานอย่างไร
- **ขั้นตอนการทำซ้ำ**: 1, 2, 3...
- **Environment**: Revit version, Windows version
- **Screenshots**: ถ้ามี

### 2. เสนอ Feature ใหม่ 💡

เปิด Issue พร้อม:

- **ปัญหาที่ต้องการแก้**: อธิบายให้ชัดเจน
- **วิธีแก้ที่เสนอ**: Feature ที่ต้องการ
- **ทางเลือกอื่น**: ถ้ามี
- **ตัวอย่างการใช้งาน**: Use case

### 3. แก้ไขโค้ด 🛠️

```bash
# 1. Fork repository
# 2. Clone fork ของคุณ
git clone https://github.com/YOUR_USERNAME/RevitElementsExporter.git

# 3. สร้าง branch ใหม่
git checkout -b feature/your-feature-name

# 4. ทำการแก้ไข
# 5. รัน tests
dotnet test

# 6. Commit
git commit -m "feat: Add your feature"

# 7. Push
git push origin feature/your-feature-name

# 8. เปิด Pull Request
```

---

## 📐 Code Style

### C# Conventions

```csharp
// ✅ Recommended
namespace RevitElementsExporter.Services
{
    public class MyService
    {
        private readonly List<string> _items;

        public MyService()
        {
            _items = new List<string>();
        }

        public void ProcessItems()
        {
            foreach (var item in _items)
            {
                Console.WriteLine(item);
            }
        }
    }
}
```

### Naming

| Type          | Style       | Example                          |
| ------------- | ----------- | -------------------------------- |
| Namespace     | PascalCase  | `RevitElementsExporter.Services` |
| Class         | PascalCase  | `ExportService`                  |
| Interface     | IPascalCase | `IExportService`                 |
| Method        | PascalCase  | `ExportToCsv()`                  |
| Property      | PascalCase  | `FilePath`                       |
| Private field | \_camelCase | `_isDarkMode`                    |
| Parameter     | camelCase   | `filePath`                       |
| Constant      | PascalCase  | `FeetToMeters`                   |

### XAML Conventions

```xml
<!-- ✅ Recommended -->
<Button x:Name="ExportBtn"
        Content="Export"
        Style="{StaticResource PrimaryButton}"
        Click="OnExportClick" />

<!-- ❌ Avoid -->
<Button x:Name="btn1" Content="Export" Style="{StaticResource PrimaryButton}" Click="OnExportClick"/>
```

---

## 🧪 Testing

### ต้องมี Tests สำหรับ

- ✅ ทุก public method ใน Services/
- ✅ Logic ที่ซับซ้อน
- ✅ Edge cases (null, empty, etc.)

### ไม่จำเป็นต้องมี Tests สำหรับ

- ❌ UI code (XAML)
- ❌ Revit API calls โดยตรง

### ตัวอย่าง Test

```csharp
public class ExportServiceTests
{
    [Fact]
    public void FormatNullable_WithValue_ReturnsFormattedString()
    {
        // Arrange
        double? value = 123.456789;

        // Act
        string result = ExportService.FormatNullable(value);

        // Assert
        Assert.Equal("123.4568", result);
    }

    [Fact]
    public void FormatNullable_WithNull_ReturnsEmptyString()
    {
        // Arrange
        double? value = null;

        // Act
        string result = ExportService.FormatNullable(value);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
```

---

## 📝 Commit Messages

ใช้ Conventional Commits:

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

### Types

| Type       | Description                |
| ---------- | -------------------------- |
| `feat`     | Feature ใหม่               |
| `fix`      | Bug fix                    |
| `docs`     | Documentation              |
| `style`    | Formatting (ไม่กระทบ code) |
| `refactor` | Refactoring                |
| `test`     | เพิ่ม/แก้ไข tests          |
| `chore`    | Maintenance                |

### Examples

```
feat(export): Add parameter export support
fix(csv): Fix delimiter format issue
docs(readme): Update installation instructions
refactor(services): Extract ExportService from main class
test(filter): Add category filter tests
```

---

## 🔍 Pull Request Checklist

ก่อนเปิด PR ตรวจสอบว่า:

- [ ] Code builds successfully (`dotnet build`)
- [ ] All tests pass (`dotnet test`)
- [ ] Code follows style guidelines
- [ ] Commit messages follow convention
- [ ] Documentation updated (if needed)
- [ ] PR description explains changes

---

## 🏗️ Project Structure

```
RevitElementsExporter/
├── RevitElementsExporter/     # Main project
│   ├── ElementsExporter.cs    # Entry point (IExternalCommand)
│   ├── ExportWindow.xaml      # Main UI
│   ├── Models/                # Data models
│   ├── Services/              # Business logic
│   └── Themes/                # UI styles
│
├── RevitElementsExporter.Tests/   # Unit tests
│
├── docs/                      # Documentation
├── Deploy.ps1                 # Deployment script
└── README.md                  # Main readme
```

---

## ❓ Questions?

- เปิด Issue สำหรับคำถาม
- หรือ Discussion สำหรับการสนทนา

---

<div align="center">

**ขอบคุณที่มีส่วนร่วม! 🙏**

</div>
