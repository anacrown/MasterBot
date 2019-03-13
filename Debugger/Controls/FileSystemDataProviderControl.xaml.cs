using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BomberMan_SuperAI.Annotations;
using CodenjoyBot.DataProvider;
using Microsoft.Win32;

namespace Debugger.Controls
{
    public partial class FileSystemDataProviderControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(FileSystemDataProvider), typeof(FileSystemDataProviderControl), new PropertyMetadata(default(FileSystemDataProvider)));

        public FileSystemDataProvider DataProvider
        {
            get => (FileSystemDataProvider)GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public FileSystemDataProviderControl()
        {
            InitializeComponent();
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
            var dialog = new OpenFileDialog { InitialDirectory = System.IO.Path.GetFullPath(WebSocketDataLogger.Instance.MainLogDir) };
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
