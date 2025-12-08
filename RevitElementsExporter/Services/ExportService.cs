using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using RevitElementsExporter.Models;

namespace RevitElementsExporter.Services
{
    /// <summary>
    /// Pure export service - no Revit dependencies, fully testable
    /// </summary>
    public static class ExportService
    {
        public const double FeetToMeters = 0.3048;

        /// <summary>
        /// Export rows to CSV format
        /// </summary>
        public static void ExportToCsv(IEnumerable<ElementExportRow> rows, string filePath, IProgress<int>? progress = null)
        {
            var rowList = rows.ToList();
            int total = rowList.Count;
            int current = 0;

            StringBuilder csvData = new();
            csvData.AppendLine("Id,Category,Family,Type,Level,LocationType,X,Y,Z,StartX,StartY,StartZ,EndX,EndY,EndZ");

            foreach (ElementExportRow row in rowList)
            {
                csvData.AppendLine(string.Join(",",
                    EscapeCsv(row.Id),
                    EscapeCsv(row.Category),
                    EscapeCsv(row.Family),
                    EscapeCsv(row.Type),
                    EscapeCsv(row.Level),
                    EscapeCsv(row.LocationType),
                    FormatNullable(row.X), FormatNullable(row.Y), FormatNullable(row.Z),
                    FormatNullable(row.StartX), FormatNullable(row.StartY), FormatNullable(row.StartZ),
                    FormatNullable(row.EndX), FormatNullable(row.EndY), FormatNullable(row.EndZ)
                ));

                current++;
                if (total > 0 && current % 100 == 0)
                {
                    progress?.Report((int)((double)current / total * 100));
                }
            }

            File.WriteAllText(filePath, csvData.ToString());
            progress?.Report(100);
        }

        /// <summary>
        /// Export rows to JSON format
        /// </summary>
        public static void ExportToJson(IEnumerable<ElementExportRow> rows, string filePath, IProgress<int>? progress = null)
        {
            progress?.Report(10);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(rows, options);
            progress?.Report(80);
            File.WriteAllText(filePath, json);
            progress?.Report(100);
        }

        /// <summary>
        /// Export rows to Excel format
        /// </summary>
        public static void ExportToExcel(IEnumerable<ElementExportRow> rows, string filePath, IProgress<int>? progress = null)
        {
            var rowList = rows.ToList();
            int total = rowList.Count;
            int current = 0;

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

            foreach (ElementExportRow row in rowList)
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

                current++;
                if (total > 0 && current % 100 == 0)
                {
                    progress?.Report((int)((double)current / total * 100));
                }
            }
            progress?.Report(100);
        }

        /// <summary>
        /// Calculate export statistics from rows
        /// </summary>
        public static ExportStats CalculateStats(IEnumerable<ElementExportRow> rows)
        {
            var list = rows.ToList();
            return new ExportStats
            {
                TotalElements = list.Count,
                PointLocations = list.Count(r => r.LocationType == "Point"),
                CurveLocations = list.Count(r => r.LocationType == "Curve" || r.LocationType == "Curve-Unbound"),
                NoLocations = list.Count(r => r.LocationType == "None"),
                CategoryCount = list.Select(r => r.Category).Where(c => !string.IsNullOrEmpty(c)).Distinct().Count()
            };
        }

        /// <summary>
        /// Filter rows by selected categories
        /// </summary>
        public static IEnumerable<ElementExportRow> FilterByCategories(
            IEnumerable<ElementExportRow> rows, 
            IEnumerable<string> selectedCategories)
        {
            var categorySet = new HashSet<string>(selectedCategories);
            return rows.Where(r => categorySet.Contains(r.Category) || string.IsNullOrEmpty(r.Category));
        }

        /// <summary>
        /// Get category summary from rows
        /// </summary>
        public static List<CategoryInfo> GetCategorySummary(IEnumerable<ElementExportRow> rows)
        {
            return rows
                .Where(r => !string.IsNullOrEmpty(r.Category))
                .GroupBy(r => r.Category)
                .Select(g => new CategoryInfo
                {
                    Name = g.Key,
                    ElementCount = g.Count(),
                    IsSelected = true
                })
                .OrderByDescending(c => c.ElementCount)
                .ToList();
        }

        // Helper methods
        public static string FormatNullable(double? value)
        {
            return value.HasValue ? value.Value.ToString("F4") : string.Empty;
        }

        public static string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // Check for characters that require quoting: comma, quote, newline, carriage return
            if (input.Contains(',') || input.Contains('"') || input.Contains('\n') || input.Contains('\r'))
            {
                return "\"" + input.Replace("\"", "\"\"") + "\"";
            }

            return input;
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
    }
}
