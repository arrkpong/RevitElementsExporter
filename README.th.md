# RevitElementsExporter

<div align="center">

![Revit](https://img.shields.io/badge/Revit-2026-blue?style=for-the-badge&logo=autodesk)
![.NET](https://img.shields.io/badge/.NET-8.0-purple?style=for-the-badge&logo=dotnet)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**ส่งออกข้อมูล Revit Elements พร้อมพิกัดและ metadata ไปยัง CSV, JSON หรือ Excel**

[English](README.md) | ภาษาไทย

</div>

---

## ✨ คุณสมบัติ

- 📊 **รองรับหลายรูปแบบ** - CSV, JSON และ Excel (.xlsx)
- 🎯 **แปลงหน่วยอัตโนมัติ** - ฟุต → เมตร
- 🎨 **UI ทันสมัย** - WPF dialog พร้อม Light/Dark mode
- 📁 **กรอง Category** - เลือก export เฉพาะ category ที่ต้องการ
- 📈 **แสดงความคืบหน้า** - Progress bar ขณะ export
- 🔍 **แสดงตัวอย่าง** - ดูจำนวน elements ก่อน export

## 📋 ข้อมูลที่ส่งออก

| คอลัมน์                | รายละเอียด                            |
| ---------------------- | ------------------------------------- |
| `Id`                   | Element ID                            |
| `Category`             | ชื่อ Category                         |
| `Family`               | ชื่อ Family (สำหรับ FamilyInstance)   |
| `Type`                 | ชื่อ Type                             |
| `Level`                | Level ที่เชื่อมโยง                    |
| `LocationType`         | Point, Curve, Curve-Unbound หรือ None |
| `X, Y, Z`              | พิกัดจุด (เมตร)                       |
| `StartX/Y/Z, EndX/Y/Z` | จุดเริ่มต้น/สิ้นสุดของเส้นโค้ง (เมตร) |

## 📁 โครงสร้างโปรเจกต์

```
RevitElementsExporter/
├── .addin                    # Revit add-in manifest
├── ElementsExporter.cs       # Main command class
├── ExportFormat.cs           # Format enum
├── ExportWindow.xaml         # WPF dialog UI
├── ExportWindow.xaml.cs      # Dialog logic
├── RevitElementsExporter.csproj
└── Themes/
    ├── Colors.xaml           # Light mode colors
    ├── ColorsDark.xaml       # Dark mode colors
    └── Styles.xaml           # Control styles
```

## 🔧 ความต้องการ

- **.NET 8 SDK** (Windows x64)
- **Autodesk Revit 2026**
- References: `RevitAPI.dll`, `RevitAPIUI.dll` จาก `C:\Program Files\Autodesk\Revit 2026\`

## 🚀 Build

```powershell
cd RevitXYZExporter
dotnet build
```

ผลลัพธ์: `RevitElementsExporter/bin/Debug/net8.0-windows/RevitElementsExporter.dll`

## 📦 ติดตั้ง / Deploy

### วิธีที่ 1: ใช้ Script (แนะนำ)

```powershell
# รันจาก root ของโปรเจกต์
.\Deploy.ps1
```

### วิธีที่ 2: Manual

1. คัดลอก `RevitElementsExporter.dll` ไปยัง `%AppData%\Autodesk\Revit\Addins\2026\`
2. คัดลอก `.addin` manifest ไปยังโฟลเดอร์เดียวกัน
3. แก้ไข path ใน `<Assembly>` ถ้าจำเป็น
4. รีสตาร์ท Revit

## 🎮 วิธีใช้งาน

1. เปิด Revit → **Add-Ins** → **External Tools** → **Export Elements**
2. เลือกรูปแบบไฟล์ (CSV / JSON / Excel)
3. กำหนด path ปลายทาง
4. คลิก **Export** ✓

## 🎨 คุณสมบัติ UI

- **ดีไซน์ทันสมัย** - Gradient headers, animations, shadow effects
- **Light/Dark Mode** - สลับธีมได้ด้วยปุ่มเดียว
- **Layout เป็นระเบียบ** - จัดกลุ่มด้วย icons

## 🛠 หมายเหตุสำหรับนักพัฒนา

- การแปลงพิกัด: `FeetToMeters = 0.3048`
- Excel export ใช้ `DocumentFormat.OpenXml` 3.1.0 (ไม่ต้องติดตั้ง Excel)
- Revit API references ตั้งค่า `Private=false` เพื่อใช้ไฟล์ที่ติดตั้ง
- Main class: `ElementsExporter` implements `IExternalCommand`

## 📄 License

MIT License - ดูไฟล์ [LICENSE](LICENSE) สำหรับรายละเอียด
