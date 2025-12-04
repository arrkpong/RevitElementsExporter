using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace RevitElementsExporter
{
    public partial class ExportWindow : Window
    {
        public string FilePath { get; private set; }
        public ExportFormat SelectedFormat { get; private set; } = ExportFormat.Csv;

        public ExportWindow(string defaultFilePath)
        {
            InitializeComponent();
            FilePathBox.Text = defaultFilePath;
            FilePath = defaultFilePath;
            FormatCombo.SelectionChanged += OnFormatChanged;
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
                FileName = Path.GetFileName(FilePathBox.Text),
                InitialDirectory = initialDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                FilePathBox.Text = dialog.FileName;
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

        private void OnFormatChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedFormat = FormatCombo.SelectedIndex switch
            {
                1 => ExportFormat.Json,
                2 => ExportFormat.Excel,
                _ => ExportFormat.Csv
            };
            FilePathBox.Text = EnsureExtensionMatchesFormat(FilePathBox.Text);
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
