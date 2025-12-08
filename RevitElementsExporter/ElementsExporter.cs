using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitElementsExporter.Models;
using RevitElementsExporter.Services;

namespace RevitElementsExporter
{
    [Transaction(TransactionMode.Manual)]
    public class ElementsExporter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Collect all element data
                var allRecords = CollectElementData(doc);
                
                // Get category summary for filter
                var categories = ExportService.GetCategorySummary(allRecords);
                
                // Calculate initial stats
                var stats = ExportService.CalculateStats(allRecords);

                // Show export window
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string defaultFilePath = System.IO.Path.Combine(desktopPath, "RevitAllElements.csv");

                var window = new ExportWindow(defaultFilePath, allRecords, categories, stats);
                var interop = new WindowInteropHelper(window) { Owner = uiapp.MainWindowHandle };
                bool? result = window.ShowDialog();
                
                if (result != true)
                {
                    return Result.Cancelled;
                }

                // Get filtered data based on selected categories
                var selectedCategories = window.GetSelectedCategories();
                var filteredRecords = ExportService.FilterByCategories(allRecords, selectedCategories);
                var recordsList = new List<ElementExportRow>(filteredRecords);

                // Export based on format
                var progress = new Progress<int>(p => { /* Progress is handled in window */ });
                
                switch (window.SelectedFormat)
                {
                    case ExportFormat.Json:
                        ExportService.ExportToJson(recordsList, window.FilePath, progress);
                        break;
                    case ExportFormat.Excel:
                        ExportService.ExportToExcel(recordsList, window.FilePath, progress);
                        break;
                    default:
                        ExportService.ExportToCsv(recordsList, window.FilePath, progress);
                        break;
                }

                TaskDialog.Show("สำเร็จ", $"Export เสร็จสมบูรณ์!\n\nไฟล์: {window.FilePath}\nจำนวน: {recordsList.Count:N0} elements");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        /// Collect element data from Revit document
        /// </summary>
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
                        x = locPoint.Point.X * ExportService.FeetToMeters;
                        y = locPoint.Point.Y * ExportService.FeetToMeters;
                        z = locPoint.Point.Z * ExportService.FeetToMeters;
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
                        startX = sp.X * ExportService.FeetToMeters;
                        startY = sp.Y * ExportService.FeetToMeters;
                        startZ = sp.Z * ExportService.FeetToMeters;
                        endX = ep.X * ExportService.FeetToMeters;
                        endY = ep.Y * ExportService.FeetToMeters;
                        endZ = ep.Z * ExportService.FeetToMeters;
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

        private static string FormatElementId(ElementId id)
        {
            return id?.ToString() ?? string.Empty;
        }
    }
}
