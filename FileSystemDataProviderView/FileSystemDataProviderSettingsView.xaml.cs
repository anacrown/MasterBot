using System.Windows;
using System.Windows.Controls;
using BotBase;
using FileSystemDataProvider;
using Microsoft.Win32;

namespace FileSystemDataProviderView
{
    /// <summary>
    /// Interaction logic for FileSystemDataProviderSettingsView.xaml
    /// </summary>
    public partial class FileSystemDataProviderSettingsView : UserControl
    {
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            "Settings", typeof(FileSystemDataProviderSettings), typeof(FileSystemDataProviderSettingsView), new PropertyMetadata(default(FileSystemDataProviderSettings)));

        public FileSystemDataProviderSettings Settings
        {
            get => (FileSystemDataProviderSettings) GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public FileSystemDataProviderSettingsView()
        {
            InitializeComponent();
        }

        private void OpenFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (!string.IsNullOrEmpty(Settings.BoardFile))
                dialog.InitialDirectory = FileSystemConfigurator.MainLogDir;
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
                Settings.BoardFile = dialog.FileName;
        }
    }
}
