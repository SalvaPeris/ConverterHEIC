using PhotoConverter.Helpers;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace PhotoConverter.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private string[] _files;

        [ObservableProperty]
        private int _filesCount;

        [ObservableProperty]
        private string _selectedDirectory;

        [ObservableProperty]
        private Visibility _selectedDirectoryVisibility = Visibility.Hidden;

        [ObservableProperty]
        private Visibility _openSelectedDirectoryButtonVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _resetButtonVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _filesProgressBarVisibility = Visibility.Collapsed;

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
               FilesCounter();
            }
        }

        [RelayCommand]
        public void StartConversion(bool removeFiles)
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (sender, e) =>
            {
                var converter = new Converter();
                var processedFilesCounter = 0;
                ConversionProgressMessage = "Convirtiendo archivos";

                if (_filesCount > 0)
                {
                    FilesProgressBarVisibility = Visibility.Visible;
                }

                foreach (var file in _files)
                {
                    try
                    {
                        string extension = Path.GetExtension(file);

                        switch (extension.ToLowerInvariant())
                        {
                            case ".heic":
                                converter.ConvertHeicToJpg(
                                    file,
                                    Path.ChangeExtension(file, ".jpg"),
                                    100);
                                break;

                            case ".cr2":
                                converter.ConvertCr2ToJpg(
                                    file,
                                    Path.ChangeExtension(file, ".jpg"),
                                    100);
                                break;

                            default:
                                continue;
                        }

                        if (removeFiles)
                            File.Delete(file);

                        processedFilesCounter++;

                        int progress = (int)Math.Clamp(
                            (processedFilesCounter * 100.0) / _files.Length,
                            0,
                            100);

                        worker.ReportProgress(progress, processedFilesCounter);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error convirtiendo {file}: {ex.Message}");
                    }
                }
            };

            worker.ProgressChanged += (sender, e) =>
            {
                ProgressBarValue = e.ProgressPercentage;
                ConversionMessage =
                    $"{e.UserState} imágenes convertidas de un total de {_files.Length} imágenes.";
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
                    FilesProgressBarVisibility = Visibility.Collapsed;
                }
            };

            worker.RunWorkerAsync();
        }

        [RelayCommand]
        public void FilesCounter()
        {
            _files = Helpers.FilesCounter.Counter(_selectedDirectory, "*.heic", "*.CR2");
            _filesCount = _files.Length;
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
