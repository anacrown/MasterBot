using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
