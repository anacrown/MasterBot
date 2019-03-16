using System.Windows;
using System.Windows.Controls;

namespace CodenjoyBot.DataProvider.FileSystemDataProvider
{
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

        private void FrameSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DataProvider.MoveToFrame((uint)e.NewValue);
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataProvider == null) return;

            PlayButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            DataProvider.RecordPlay();
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataProvider == null) return;

            StopButton.IsEnabled = false;
            PlayButton.IsEnabled = true;

            DataProvider.RecordStop();
        }
    }
}
