namespace RevitElementsExporter.Models
{
    /// <summary>
    /// Represents a single element row for export
    /// </summary>
    public class ElementExportRow
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

    /// <summary>
    /// Category info for filtering
    /// </summary>
    public class CategoryInfo
    {
        public string Name { get; set; } = string.Empty;
        public int ElementCount { get; set; }
        public bool IsSelected { get; set; } = true;

        public override string ToString() => $"{Name} ({ElementCount})";
    }

    /// <summary>
    /// Export statistics
    /// </summary>
    public class ExportStats
    {
        public int TotalElements { get; set; }
        public int PointLocations { get; set; }
        public int CurveLocations { get; set; }
        public int NoLocations { get; set; }
        public int CategoryCount { get; set; }
    }
}
