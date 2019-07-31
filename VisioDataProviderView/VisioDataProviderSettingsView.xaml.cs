using System.Windows;
using System.Windows.Controls;
using BotBase;
using Microsoft.Win32;
using VisioDataProvider;

namespace VisioDataProviderView
{
    public partial class VisioDataProviderSettingsView : UserControl
    {
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            "Settings", typeof(VisioDataProviderSettings), typeof(VisioDataProviderSettingsView), new PropertyMetadata(default(VisioDataProviderSettings)));

        public VisioDataProviderSettings Settings
        {
            get => (VisioDataProviderSettings) GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public VisioDataProviderSettingsView()
        {
            InitializeComponent();
        }

        private void OpenFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (!string.IsNullOrEmpty(Settings.VisioFile))
                dialog.InitialDirectory = FileSystemConfigurator.MainLogDir;
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
                Settings.VisioFile = dialog.FileName;
        }
    }
}
