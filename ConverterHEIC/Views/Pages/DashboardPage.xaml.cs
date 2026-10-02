using PhotoConverter.ViewModels.Pages;
using System.Diagnostics;
using Wpf.Ui.Controls;

namespace PhotoConverter.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
            ConvertButton.Visibility = Visibility.Hidden;
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectDirectory();
            HeicFilesCount.Content = $"{ViewModel.FilesCount} imágenes encontradas.";
            if(ViewModel.FilesCount > 0)
            {
                ConvertButton.Visibility = Visibility.Visible;
                RemoveFileCheckBox.Visibility = Visibility.Visible;
                AddDateCheckBox.Visibility = Visibility.Visible;
            }
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            HeicFilesCount.Content = string.Empty;
            RemoveFileCheckBox.Visibility = Visibility.Collapsed;
            AddDateCheckBox.Visibility = Visibility.Collapsed;
            ConvertButton.Visibility= Visibility.Collapsed;
            HeicFilesCount.Visibility = Visibility.Collapsed;
            SelectFolderButton.Visibility = Visibility.Collapsed;

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
            ViewModel.Files = Array.Empty<string>();
            ViewModel.SelectedDirectory = string.Empty;
            ViewModel.ConversionMessage = string.Empty;
            HeicFilesCount.Content = string.Empty;
            ViewModel.ProgressBarValue = 0;
            
            ViewModel.SelectedDirectoryVisibility = Visibility.Hidden;
            ViewModel.OpenSelectedDirectoryButtonVisibility = Visibility.Hidden;
            ViewModel.ResetButtonVisibility = Visibility.Hidden;
            ConvertButton.Visibility = Visibility.Hidden;
            RemoveFileCheckBox.Visibility = Visibility.Hidden;
            AddDateCheckBox.Visibility = Visibility.Hidden;
            ConvertingFiles.Visibility = Visibility.Hidden;

            SelectFolderButton.Visibility = Visibility.Visible;

        }
    }
}
