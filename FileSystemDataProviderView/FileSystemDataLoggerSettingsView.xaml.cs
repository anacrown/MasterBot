using System.Windows;
using System.Windows.Controls;
using FileSystemDataProvider;

namespace FileSystemDataProviderView
{
    /// <summary>
    /// Логика взаимодействия для FileSystemDataLoggerSettingsView.xaml
    /// </summary>
    public partial class FileSystemDataLoggerSettingsView : UserControl
    {
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            "Settings", typeof(FileSystemDataLoggerSettings), typeof(FileSystemDataLoggerSettingsView), new PropertyMetadata(default(FileSystemDataLoggerSettings)));

        public FileSystemDataLoggerSettings Settings
        {
            get => (FileSystemDataLoggerSettings) GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }
        public FileSystemDataLoggerSettingsView()
        {
            InitializeComponent();
        }
    }
}
