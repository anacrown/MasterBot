using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using FileSystemDataProvider;
using FileSystemDataProviderView.Annotations;
using Microsoft.Win32;

namespace FileSystemDataProviderView
{
    public partial class FileSystemDataProviderControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(FileSystemDataProvider.FileSystemDataProvider), typeof(FileSystemDataProviderControl), new PropertyMetadata(default(FileSystemDataProvider.FileSystemDataProvider)));

        public FileSystemDataProvider.FileSystemDataProvider DataProvider
        {
            get => (FileSystemDataProvider.FileSystemDataProvider)GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public FileSystemDataProviderControl()
        {
            InitializeComponent();
        }

        public FileSystemDataProviderControl(FileSystemDataProvider.FileSystemDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }

        public string Dir
        {
            get => DataProvider?.BoardFile;
            set
            {
                if (value == DataProvider?.BoardFile) return;
                if (DataProvider != null) DataProvider.BoardFile = value;
                OnPropertyChanged();
            }
        }

        private void OpenFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { InitialDirectory = System.IO.Path.GetFullPath(FileSystemDataLogger.MainLogDir) };
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                Dir = dialog.FileName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
