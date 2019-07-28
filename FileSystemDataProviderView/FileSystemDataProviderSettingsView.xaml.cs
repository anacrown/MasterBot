using System.Windows;
using System.Windows.Controls;
using FileSystemDataProvider;
using Microsoft.Win32;

namespace FileSystemDataProviderView
{
    /// <summary>
    /// Interaction logic for FileSystemDataProviderSettingsView.xaml
    /// </summary>
    public partial class FileSystemDataProviderSettingsView : UserControl
    {
        public static readonly DependencyProperty DataProviderSettingsProperty = DependencyProperty.Register(
            "DataProviderSettings", typeof(FileSystemDataProviderSettings), typeof(FileSystemDataProviderSettingsView), new PropertyMetadata(default(FileSystemDataProviderSettings)));

        public FileSystemDataProviderSettings DataProviderSettings
        {
            get => (FileSystemDataProviderSettings) GetValue(DataProviderSettingsProperty);
            set => SetValue(DataProviderSettingsProperty, value);
        }

        public FileSystemDataProviderSettingsView()
        {
            InitializeComponent();
        }

        private void OpenFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { InitialDirectory = System.IO.Path.GetFullPath(FileSystemDataLogger.MainLogDir) };
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
                DataProviderSettings.BoardFile = dialog.FileName;

        }
    }
}
