using System;
using System.IO;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

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

            StringBuilder csvData = new();
            csvData.AppendLine("Id,Category,Family,Type,Level,LocationType,X,Y,Z,StartX,StartY,StartZ,EndX,EndY,EndZ");

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

                Location loc = e.Location;
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
                            // Skip unbounded curves (e.g., grid lines) to avoid endpoint exceptions.
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

                csvData.AppendLine(string.Join(", ",
                    EscapeCsv(FormatElementId(e.Id)),
                    EscapeCsv(categoryName),
                    EscapeCsv(familyName),
                    EscapeCsv(typeName),
                    EscapeCsv(levelName),
                    locationType,
                    FormatNullable(x), FormatNullable(y), FormatNullable(z),
                    FormatNullable(startX), FormatNullable(startY), FormatNullable(startZ),
                    FormatNullable(endX), FormatNullable(endY), FormatNullable(endZ)
                ));
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "RevitAllElements.csv");

            try
            {
                File.WriteAllText(filePath, csvData.ToString());
                TaskDialog.Show("Success", $"Export completed.\nFile: {filePath}");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
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
    }
}
