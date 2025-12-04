# RevitElementsExporter (ภาษาไทย)

Add-in สำหรับ Revit (net8.0-windows) ที่ส่งออกข้อมูล element ทั้งหมดพร้อมพิกัดและข้อมูลกำกับอื่น ๆ ผ่านหน้าต่าง WPF เลือกฟอร์แมตไฟล์ได้ (CSV, JSON, Excel) และตำแหน่งเซฟไฟล์ พิกัดถูกแปลงจากหน่วยฟุตภายในของ Revit เป็นเมตร

## คุณสมบัติ
- ส่งออก instance (ไม่รวม element type) พร้อมคอลัมน์: Id, Category, Family, Type, Level, LocationType, XYZ และ Start/End XYZ สำหรับเส้นโค้ง
- รองรับ 3 ฟอร์แมต: CSV, JSON, Excel (.xlsx ผ่าน OpenXML)
- หน้าต่าง WPF ให้เลือกฟอร์แมตและที่อยู่ไฟล์ (ค่าเริ่มต้น `RevitAllElements.csv` ที่ Desktop)
- จัดการเส้นโค้งแบบไม่ bound โดยใส่ `LocationType` เป็น `Curve-Unbound`

## โครงสร้างโปรเจ็กต์
- `RevitElementsExporter.sln` – ไฟล์ solution
- `RevitElementsExporter/` – ซอร์ส add-in (`ExportCoordinates.cs`, `ExportWindow.xaml`), โปรเจ็กต์ (`RevitElementsExporter.csproj`), ทรัพยากรธีม (`Themes/`), และไฟล์ manifest (`.addin`)
- `bin/`, `obj/` – ไฟล์ build ที่สร้างอัตโนมัติ

## ความต้องการ
- .NET 8 SDK
- Windows x64
- Autodesk Revit 2026; โปรเจ็กต์อ้าง `RevitAPI.dll` และ `RevitAPIUI.dll` จาก `C:\Program Files\Autodesk\Revit 2026\`

## วิธี Build
จากโฟลเดอร์รากของ repo:
```powershell
dotnet build
```
ไฟล์ DLL จะอยู่ที่ `RevitElementsExporter/bin/Debug/net8.0-windows/RevitElementsExporter.dll`

## การติดตั้ง / Deploy
1. คัดลอก `RevitElementsExporter/bin/Debug/net8.0-windows/RevitElementsExporter.dll` ไปยัง `%AppData%\Autodesk\Revit\Addins\2026\` (หรือเวอร์ชันที่ใช้งาน)
2. คัดลอก `RevitElementsExporter/.addin` ไปที่โฟลเดอร์เดียวกัน และอัปเดต `<Assembly>` หาก path ไม่ตรง
3. รีสตาร์ท Revit

## วิธีใช้งาน
1. ใน Revit ไปที่ Add-Ins → External Tools → Revit Elements Exporter
2. ในหน้าต่าง:
   - **Format**: เลือก CSV / JSON / Excel (.xlsx)
   - **File path**: ค่าเริ่มต้นที่ Desktop กด Browse เพื่อเปลี่ยน
3. กด **Export** เพื่อบันทึกไฟล์ (พิกัดเป็นเมตร)

## คอลัมน์ที่ส่งออก
`Id, Category, Family, Type, Level, LocationType, X, Y, Z, StartX, StartY, StartZ, EndX, EndY, EndZ`
- `X/Y/Z`: พิกัดจุด (เมตร) เมื่อ `LocationType=Point`
- `Start*/End*`: จุดปลายเส้นโค้ง (เมตร) เมื่อ `LocationType=Curve`; เส้นไม่ bound จะถูกระบุ `Curve-Unbound`

## โน้ตสำหรับนักพัฒนา
- การแปลงหน่วยใช้คงที่ `FeetToMeters` ใน `ExportCoordinates.cs`
- UI อยู่ใน `ExportWindow.xaml` และธีมในโฟลเดอร์ `Themes/`
- การส่งออก Excel ใช้ `DocumentFormat.OpenXml` (3.1.0) ไม่ต้องใช้ interop
- อ้าง Revit API ด้วย `Private=false` เพื่อใช้ DLL จากการติดตั้ง Revit; ปรับ hint path หากเปลี่ยนเวอร์ชัน Revit
- หากจะเพิ่มชุดทดสอบ ให้สร้างโปรเจ็กต์ทดสอบแยกและ mock Revit API (ไม่โหลด DLL จริงในเทสต์)
