using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using RevitElementsExporter.Models;
using RevitElementsExporter.Services;

namespace RevitElementsExporter
{
    public partial class ExportWindow : Window
    {
        public string FilePath { get; private set; }
        public ExportFormat SelectedFormat { get; private set; } = ExportFormat.Csv;
        
        private bool _isDarkMode = false;
        private readonly List<ElementExportRow> _allRecords;
        private readonly List<CategoryInfo> _categories;
        private ExportStats _currentStats;

        public ExportWindow(string defaultFilePath) 
            : this(defaultFilePath, new List<ElementExportRow>(), new List<CategoryInfo>(), new ExportStats())
        {
        }

        public ExportWindow(
            string defaultFilePath, 
            List<ElementExportRow> records, 
            List<CategoryInfo> categories,
            ExportStats stats)
        {
            InitializeComponent();
            
            FilePathBox.Text = defaultFilePath;
            FilePath = defaultFilePath;
            
            _allRecords = records;
            _categories = categories;
            _currentStats = stats;
            
            FormatCombo.SelectionChanged += OnFormatChanged;
            
            // Setup category list
            CategoryList.ItemsSource = _categories;
            
            // Update UI with stats
            UpdateStatsDisplay();
            UpdateSelectedCategoryCount();
        }

        public IEnumerable<string> GetSelectedCategories()
        {
            return _categories.Where(c => c.IsSelected).Select(c => c.Name);
        }

        private void UpdateStatsDisplay()
        {
            TotalElementsText.Text = _currentStats.TotalElements.ToString("N0");
            PointCountText.Text = _currentStats.PointLocations.ToString("N0");
            CurveCountText.Text = _currentStats.CurveLocations.ToString("N0");
            CategoryCountText.Text = _currentStats.CategoryCount.ToString("N0");
        }

        private void UpdateSelectedCategoryCount()
        {
            int selectedCount = _categories.Count(c => c.IsSelected);
            SelectedCategoryText.Text = $"เลือก {selectedCount} / {_categories.Count} categories";
            
            // Recalculate stats based on selected categories
            var selectedCategoryNames = GetSelectedCategories().ToHashSet();
            var filteredRecords = _allRecords.Where(r => 
                selectedCategoryNames.Contains(r.Category) || string.IsNullOrEmpty(r.Category)).ToList();
            
            _currentStats = ExportService.CalculateStats(filteredRecords);
            UpdateStatsDisplay();
        }

        private void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            string initialDirectory = Path.GetDirectoryName(FilePathBox.Text) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(initialDirectory) || !Directory.Exists(initialDirectory))
            {
                initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json|Excel Workbook (*.xlsx)|*.xlsx",
                FilterIndex = FormatCombo.SelectedIndex + 1, // Sync with format selection
                FileName = Path.GetFileName(FilePathBox.Text),
                InitialDirectory = initialDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                FilePathBox.Text = dialog.FileName;
                // Update format based on file extension
                string ext = Path.GetExtension(dialog.FileName).ToLowerInvariant();
                FormatCombo.SelectedIndex = ext switch
                {
                    ".json" => 1,
                    ".xlsx" => 2,
                    _ => 0
                };
            }
        }

        private void OnExportClick(object sender, RoutedEventArgs e)
        {
            string path = FilePathBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show(this, "กรุณาระบุที่อยู่ไฟล์ปลายทาง", "Missing file path", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            FilePath = EnsureExtensionMatchesFormat(path);
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnFormatChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFormat = FormatCombo.SelectedIndex switch
            {
                1 => ExportFormat.Json,
                2 => ExportFormat.Excel,
                _ => ExportFormat.Csv
            };
            FilePathBox.Text = EnsureExtensionMatchesFormat(FilePathBox.Text);
        }

        private void OnSelectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var cat in _categories)
            {
                cat.IsSelected = true;
            }
            CategoryList.Items.Refresh();
            UpdateSelectedCategoryCount();
        }

        private void OnSelectNoneClick(object sender, RoutedEventArgs e)
        {
            foreach (var cat in _categories)
            {
                cat.IsSelected = false;
            }
            CategoryList.Items.Refresh();
            UpdateSelectedCategoryCount();
        }

        private void OnCategorySelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectedCategoryCount();
        }

        private void OnThemeToggleClick(object sender, RoutedEventArgs e)
        {
            _isDarkMode = !_isDarkMode;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (_isDarkMode)
            {
                ThemeIcon.Text = "☀️";
            }
            else
            {
                ThemeIcon.Text = "🌙";
            }
            UpdateThemeColors();
        }

        private void UpdateThemeColors()
        {
            if (_isDarkMode)
            {
                // Dark mode colors - backgrounds
                Background = new SolidColorBrush(Color.FromRgb(15, 23, 42));
                MainCard.Background = new SolidColorBrush(Color.FromRgb(30, 41, 59));
                MainCard.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85));
                InfoBox.Background = new SolidColorBrush(Color.FromRgb(30, 58, 95));
                ContentArea.Background = new SolidColorBrush(Color.FromRgb(15, 23, 42));
                StatsCard.Background = new SolidColorBrush(Color.FromRgb(6, 78, 59));
                StatsCard.BorderBrush = new SolidColorBrush(Color.FromRgb(4, 120, 87));
                
                // Dark mode - text colors
                var lightText = new SolidColorBrush(Color.FromRgb(241, 245, 249)); // #F1F5F9
                var mutedText = new SolidColorBrush(Color.FromRgb(148, 163, 184)); // #94A3B8
                
                FormatLabel.Foreground = lightText;
                FilePathLabel.Foreground = lightText;
                CategoryLabel.Foreground = lightText;
                SelectedCategoryText.Foreground = mutedText;
                
                // TextBox and ComboBox
                FilePathBox.Background = new SolidColorBrush(Color.FromRgb(15, 23, 42));
                FilePathBox.Foreground = lightText;
                FilePathBox.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85));
                
                FormatCombo.Background = new SolidColorBrush(Color.FromRgb(15, 23, 42));
                FormatCombo.Foreground = lightText;
                FormatCombo.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85));
            }
            else
            {
                // Light mode colors - backgrounds
                Background = new SolidColorBrush(Color.FromRgb(248, 250, 252));
                MainCard.Background = new SolidColorBrush(Colors.White);
                MainCard.BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240));
                InfoBox.Background = new SolidColorBrush(Color.FromRgb(240, 249, 255));
                ContentArea.Background = Brushes.Transparent;
                StatsCard.Background = new SolidColorBrush(Color.FromRgb(236, 253, 245));
                StatsCard.BorderBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                
                // Light mode - text colors
                var darkText = new SolidColorBrush(Color.FromRgb(30, 41, 59)); // #1E293B
                var mutedText = new SolidColorBrush(Color.FromRgb(100, 116, 139)); // #64748B
                
                FormatLabel.Foreground = darkText;
                FilePathLabel.Foreground = darkText;
                CategoryLabel.Foreground = darkText;
                SelectedCategoryText.Foreground = mutedText;
                
                // TextBox and ComboBox
                FilePathBox.Background = Brushes.White;
                FilePathBox.Foreground = darkText;
                FilePathBox.BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240));
                
                FormatCombo.Background = Brushes.White;
                FormatCombo.Foreground = darkText;
                FormatCombo.BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240));
            }
        }

        private string EnsureExtensionMatchesFormat(string path)
        {
            string ext = SelectedFormat switch
            {
                ExportFormat.Json => ".json",
                ExportFormat.Excel => ".xlsx",
                _ => ".csv"
            };
            try
            {
                string updated = Path.ChangeExtension(path, ext);
                return string.IsNullOrWhiteSpace(updated) ? path : updated;
            }
            catch
            {
                return path;
            }
        }
    }
}
