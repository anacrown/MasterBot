using System.Windows;
using System.Windows.Controls;

namespace CodenjoyBot.DataProvider.FileSystemDataProvider
{
    /// <summary>
    /// Interaction logic for FileSystemDataProviderDebugControl.xaml
    /// </summary>
    public partial class FileSystemDataProviderDebugControl : UserControl
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(FileSystemDataProvider), typeof(FileSystemDataProviderDebugControl), new PropertyMetadata(default(FileSystemDataProvider)));

        public FileSystemDataProvider DataProvider
        {
            get => (FileSystemDataProvider) GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public FileSystemDataProviderDebugControl()
        {
            InitializeComponent();
        }

        public FileSystemDataProviderDebugControl(FileSystemDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }
    }
}
