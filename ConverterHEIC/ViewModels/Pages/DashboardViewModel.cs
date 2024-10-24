using ConverterHEIC.Helpers;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace ConverterHEIC.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private string[] _heicFiles;

        [ObservableProperty]
        private int _heicFilesCount;

        [ObservableProperty]
        private string _selectedDirectory;

        [ObservableProperty]
        private Visibility _selectedDirectoryVisibility = Visibility.Hidden;

        [ObservableProperty]
        private Visibility _openSelectedDirectoryButtonVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _resetButtonVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private double _progressBarValue;

        [ObservableProperty]
        private string _conversionMessage;

        [ObservableProperty]
        private string _conversionProgressMessage;

        [RelayCommand]
        public void SelectDirectory()
        {
            var folderDialog = new VistaFolderBrowserDialog();
            folderDialog.Description = "Select a folder";
            folderDialog.UseDescriptionForTitle = true;

            if (folderDialog.ShowDialog() == true)
            {
                string selectedPath = folderDialog.SelectedPath;
                SelectedDirectory = selectedPath;
                SelectedDirectoryVisibility = Visibility.Visible;
                HeicFilesCounter();
            }
        }

        [RelayCommand]
        public void StartConversion(bool removeFiles)
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (sender, e) =>
            {
                var converter = new HeicConverter();
                var processedFilesCounter = 0;
                ConversionProgressMessage = "Convirtiendo archivos";

                foreach (var file in _heicFiles)
                {
                    try
                    {
                        converter.ConvertHeicToJpg(file, Path.ChangeExtension(file, ".jpg"), 100);
                        if (removeFiles)
                            File.Delete(file);

                        processedFilesCounter++;
                        worker.ReportProgress((processedFilesCounter * 100) / _heicFiles.Length, processedFilesCounter);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            };

            worker.ProgressChanged += (sender, e) =>
            {
                ProgressBarValue = e.ProgressPercentage;
                ConversionMessage = $"{e.UserState} imágenes convertidas de un total de {_heicFiles.Length} imágenes.";
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    MessageBox.Show($"Error: {e.Error.Message}");
                }
                else
                {
                    ConversionProgressMessage = "Conversión finalizada";
                    OpenSelectedDirectoryButtonVisibility = Visibility.Visible;
                    ResetButtonVisibility = Visibility.Visible;
                }
            };

            worker.RunWorkerAsync();
        }

        [RelayCommand]
        public void HeicFilesCounter()
        {
            _heicFiles = FilesCounter.Counter(_selectedDirectory, "*.heic");
            _heicFilesCount = _heicFiles.Length;
        }

        [RelayCommand]
        public void OpenSelectedDirectory()
        {
            if (!string.IsNullOrEmpty(SelectedDirectory))
            {
                Process.Start("explorer.exe", SelectedDirectory);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un directorio primero.");
            }
        }
    }
}
