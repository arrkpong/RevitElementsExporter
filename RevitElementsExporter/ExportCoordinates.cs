using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace RevitElementsExporter
{
    [Transaction(TransactionMode.Manual)]
    public class ExportCoordinates : IExternalCommand
    {
        private const double FeetToMeters = 0.3048;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string defaultFilePath = Path.Combine(desktopPath, "RevitAllElements.csv");

            var window = new ExportWindow(defaultFilePath);
            var interop = new WindowInteropHelper(window) { Owner = uiapp.MainWindowHandle };
            bool? result = window.ShowDialog();
            if (result != true)
            {
                return Result.Cancelled;
            }

            try
            {
                var records = CollectElementData(doc);
                switch (window.SelectedFormat)
                {
                    case ExportFormat.Json:
                        ExportToJson(records, window.FilePath);
                        break;
                    case ExportFormat.Excel:
                        ExportToExcel(records, window.FilePath);
                        break;
                    default:
                        ExportToCsv(records, window.FilePath);
                        break;
                }

                TaskDialog.Show("Success", $"Export completed.\nFile: {window.FilePath}");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private static List<ElementExportRow> CollectElementData(Document doc)
        {
            var data = new List<ElementExportRow>();

            var allInstances = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (Element e in allInstances)
            {
                string categoryName = e.Category?.Name ?? string.Empty;
                string familyName = e is FamilyInstance fi ? fi.Symbol?.Family?.Name ?? string.Empty : string.Empty;
                string typeName = e.Name ?? string.Empty;
                string levelName = doc.GetElement(e.LevelId) is Level level ? level.Name : string.Empty;

                string locationType;
                double? x = null, y = null, z = null;
                double? startX = null, startY = null, startZ = null;
                double? endX = null, endY = null, endZ = null;

                Autodesk.Revit.DB.Location loc = e.Location;
                switch (loc)
                {
                    case LocationPoint locPoint:
                        locationType = "Point";
                        x = locPoint.Point.X * FeetToMeters;
                        y = locPoint.Point.Y * FeetToMeters;
                        z = locPoint.Point.Z * FeetToMeters;
                        break;
                    case LocationCurve locCurve:
                        locationType = "Curve";
                        Curve curve = locCurve.Curve;
                        if (!curve.IsBound)
                        {
                            locationType = "Curve-Unbound";
                            break;
                        }
                        XYZ sp = curve.GetEndPoint(0);
                        XYZ ep = curve.GetEndPoint(1);
                        startX = sp.X * FeetToMeters;
                        startY = sp.Y * FeetToMeters;
                        startZ = sp.Z * FeetToMeters;
                        endX = ep.X * FeetToMeters;
                        endY = ep.Y * FeetToMeters;
                        endZ = ep.Z * FeetToMeters;
                        break;
                    default:
                        locationType = "None";
                        break;
                }

                data.Add(new ElementExportRow
                {
                    Id = FormatElementId(e.Id),
                    Category = categoryName,
                    Family = familyName,
                    Type = typeName,
                    Level = levelName,
                    LocationType = locationType,
                    X = x,
                    Y = y,
                    Z = z,
                    StartX = startX,
                    StartY = startY,
                    StartZ = startZ,
                    EndX = endX,
                    EndY = endY,
                    EndZ = endZ
                });
            }

            return data;
        }

        private static void ExportToCsv(IEnumerable<ElementExportRow> rows, string filePath)
        {
            StringBuilder csvData = new();
            csvData.AppendLine("Id,Category,Family,Type,Level,LocationType,X,Y,Z,StartX,StartY,StartZ,EndX,EndY,EndZ");

            foreach (ElementExportRow row in rows)
            {
                csvData.AppendLine(string.Join(", ",
                    EscapeCsv(row.Id),
                    EscapeCsv(row.Category),
                    EscapeCsv(row.Family),
                    EscapeCsv(row.Type),
                    EscapeCsv(row.Level),
                    row.LocationType,
                    FormatNullable(row.X), FormatNullable(row.Y), FormatNullable(row.Z),
                    FormatNullable(row.StartX), FormatNullable(row.StartY), FormatNullable(row.StartZ),
                    FormatNullable(row.EndX), FormatNullable(row.EndY), FormatNullable(row.EndZ)
                ));
            }

            File.WriteAllText(filePath, csvData.ToString());
        }

        private static void ExportToJson(IEnumerable<ElementExportRow> rows, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(rows, options);
            File.WriteAllText(filePath, json);
        }

        private static void ExportToExcel(IEnumerable<ElementExportRow> rows, string filePath)
        {
            using SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);
            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            SheetData sheetData = new();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            Sheets sheets = document.WorkbookPart!.Workbook.AppendChild(new Sheets());
            Sheet sheet = new()
            {
                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Elements"
            };
            sheets.Append(sheet);

            string[] headers = { "Id", "Category", "Family", "Type", "Level", "LocationType", "X", "Y", "Z", "StartX", "StartY", "StartZ", "EndX", "EndY", "EndZ" };
            sheetData.AppendChild(CreateRow(headers.Select(CreateTextCell)));

            foreach (ElementExportRow row in rows)
            {
                var cells = new List<Cell>
                {
                    CreateTextCell(row.Id),
                    CreateTextCell(row.Category),
                    CreateTextCell(row.Family),
                    CreateTextCell(row.Type),
                    CreateTextCell(row.Level),
                    CreateTextCell(row.LocationType),
                    CreateNumberCell(row.X),
                    CreateNumberCell(row.Y),
                    CreateNumberCell(row.Z),
                    CreateNumberCell(row.StartX),
                    CreateNumberCell(row.StartY),
                    CreateNumberCell(row.StartZ),
                    CreateNumberCell(row.EndX),
                    CreateNumberCell(row.EndY),
                    CreateNumberCell(row.EndZ)
                };

                sheetData.AppendChild(CreateRow(cells));
            }
        }

        private static string FormatNullable(double? value)
        {
            return value.HasValue ? value.Value.ToString("F4") : string.Empty;
        }

        private static string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (input.Contains(',') || input.Contains('\"'))
            {
                return "\"" + input.Replace("\"", "\"\"") + "\"";
            }

            return input;
        }

        private static string FormatElementId(ElementId id)
        {
            // ElementId.IntegerValue caused a compile error on this environment; ToString works across versions.
            return id?.ToString() ?? string.Empty;
        }

        private static Row CreateRow(IEnumerable<Cell> cells)
        {
            Row row = new();
            foreach (Cell cell in cells)
            {
                row.Append(cell);
            }

            return row;
        }

        private static Cell CreateTextCell(string? value)
        {
            return new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(value ?? string.Empty)
            };
        }

        private static Cell CreateNumberCell(double? value)
        {
            if (!value.HasValue)
            {
                return new Cell { DataType = CellValues.String, CellValue = new CellValue(string.Empty) };
            }

            return new Cell
            {
                DataType = CellValues.Number,
                CellValue = new CellValue(value.Value.ToString("F4", CultureInfo.InvariantCulture))
            };
        }

        private class ElementExportRow
        {
            public string Id { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Family { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Level { get; set; } = string.Empty;
            public string LocationType { get; set; } = string.Empty;
            public double? X { get; set; }
            public double? Y { get; set; }
            public double? Z { get; set; }
            public double? StartX { get; set; }
            public double? StartY { get; set; }
            public double? StartZ { get; set; }
            public double? EndX { get; set; }
            public double? EndY { get; set; }
            public double? EndZ { get; set; }
        }
    }
}
