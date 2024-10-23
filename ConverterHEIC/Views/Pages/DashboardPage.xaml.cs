using ConverterHEIC.ViewModels.Pages;
using System.Diagnostics;
using Wpf.Ui.Controls;

namespace ConverterHEIC.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
            FilesProgressBar.Visibility = Visibility.Collapsed;
            ConvertButton.Visibility = Visibility.Hidden;
            DataContext = ViewModel;
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectDirectory();
            HeicFilesCount.Content = $"{ViewModel.HeicFilesCount} imágenes .heic encontradas.";
            if(ViewModel.HeicFilesCount > 0)
            {
                ConvertButton.Visibility = Visibility.Visible;
                RemoveFileCheckBox.Visibility = Visibility.Visible;
            }
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            HeicFilesCount.Content = string.Empty;
            RemoveFileCheckBox.Visibility = Visibility.Collapsed;
            ConvertButton.Visibility= Visibility.Collapsed;
            HeicFilesCount.Visibility = Visibility.Collapsed;
            SelectFolderButton.Visibility = Visibility.Collapsed;

            FilesProgressBar.Visibility = Visibility.Visible;

            ViewModel.StartConversion(RemoveFileCheckBox.IsChecked ?? false);
        }

        private void OpenSelectedDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.SelectedDirectory))
            {
                Process.Start("explorer.exe", ViewModel.SelectedDirectory);
            }
            else
            {
                System.Windows.MessageBox.Show("Por favor, selecciona un directorio primero.");
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.HeicFiles = Array.Empty<string>();
            ViewModel.SelectedDirectory = string.Empty;
            ViewModel.ConversionMessage = string.Empty;
            HeicFilesCount.Content = string.Empty;
            ViewModel.ProgressBarValue = 0;
            
            ViewModel.SelectedDirectoryVisibility = Visibility.Hidden;
            ViewModel.OpenSelectedDirectoryButtonVisibility = Visibility.Hidden;
            ViewModel.ResetButtonVisibility = Visibility.Hidden;

            FilesProgressBar.Visibility = Visibility.Hidden;
            ConvertButton.Visibility = Visibility.Hidden;
            RemoveFileCheckBox.Visibility = Visibility.Hidden;
            ConvertingFiles.Visibility = Visibility.Hidden;

            SelectFolderButton.Visibility = Visibility.Visible;

        }
    }
}
