using RevitElementsExporter.Models;
using RevitElementsExporter.Services;

namespace RevitElementsExporter.Tests;

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

    [Fact]
    public void EscapeCsv_WithComma_ReturnsQuotedString()
    {
        // Arrange
        string input = "Hello, World";

        // Act
        string result = ExportService.EscapeCsv(input);

        // Assert
        Assert.Equal("\"Hello, World\"", result);
    }

    [Fact]
    public void EscapeCsv_WithQuote_EscapesQuote()
    {
        // Arrange
        string input = "Say \"Hello\"";

        // Act
        string result = ExportService.EscapeCsv(input);

        // Assert
        Assert.Equal("\"Say \"\"Hello\"\"\"", result);
    }

    [Fact]
    public void EscapeCsv_WithNormalText_ReturnsUnchanged()
    {
        // Arrange
        string input = "Hello World";

        // Act
        string result = ExportService.EscapeCsv(input);

        // Assert
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void EscapeCsv_WithEmpty_ReturnsEmpty()
    {
        // Act
        string result = ExportService.EscapeCsv(string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void CalculateStats_ReturnsCorrectCounts()
    {
        // Arrange
        var rows = new List<ElementExportRow>
        {
            new() { Id = "1", Category = "Walls", LocationType = "Point" },
            new() { Id = "2", Category = "Walls", LocationType = "Point" },
            new() { Id = "3", Category = "Doors", LocationType = "Curve" },
            new() { Id = "4", Category = "Windows", LocationType = "None" },
            new() { Id = "5", Category = "Windows", LocationType = "Curve-Unbound" },
        };

        // Act
        var stats = ExportService.CalculateStats(rows);

        // Assert
        Assert.Equal(5, stats.TotalElements);
        Assert.Equal(2, stats.PointLocations);
        Assert.Equal(2, stats.CurveLocations); // Curve + Curve-Unbound
        Assert.Equal(1, stats.NoLocations);
        Assert.Equal(3, stats.CategoryCount); // Walls, Doors, Windows
    }

    [Fact]
    public void GetCategorySummary_ReturnsSortedByCount()
    {
        // Arrange
        var rows = new List<ElementExportRow>
        {
            new() { Category = "Doors" },
            new() { Category = "Walls" },
            new() { Category = "Walls" },
            new() { Category = "Walls" },
            new() { Category = "Windows" },
            new() { Category = "Windows" },
        };

        // Act
        var summary = ExportService.GetCategorySummary(rows);

        // Assert
        Assert.Equal(3, summary.Count);
        Assert.Equal("Walls", summary[0].Name);
        Assert.Equal(3, summary[0].ElementCount);
        Assert.Equal("Windows", summary[1].Name);
        Assert.Equal(2, summary[1].ElementCount);
        Assert.Equal("Doors", summary[2].Name);
        Assert.Equal(1, summary[2].ElementCount);
    }

    [Fact]
    public void FilterByCategories_FiltersCorrectly()
    {
        // Arrange
        var rows = new List<ElementExportRow>
        {
            new() { Id = "1", Category = "Walls" },
            new() { Id = "2", Category = "Doors" },
            new() { Id = "3", Category = "Windows" },
            new() { Id = "4", Category = "Walls" },
            new() { Id = "5", Category = "" }, // Empty category should be included
        };
        var selectedCategories = new[] { "Walls", "Doors" };

        // Act
        var filtered = ExportService.FilterByCategories(rows, selectedCategories).ToList();

        // Assert
        Assert.Equal(4, filtered.Count); // 2 Walls + 1 Door + 1 Empty
        Assert.DoesNotContain(filtered, r => r.Category == "Windows");
    }

    [Fact]
    public void FeetToMeters_HasCorrectValue()
    {
        // Assert
        Assert.Equal(0.3048, ExportService.FeetToMeters);
    }
}
