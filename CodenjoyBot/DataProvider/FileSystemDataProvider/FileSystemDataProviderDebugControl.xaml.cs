using System;
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

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property != DataProviderProperty) return;
            if (e.OldValue is FileSystemDataProvider oldValue) oldValue.TimeChanged -= DataProviderOnTimeChanged;
            if (e.NewValue is FileSystemDataProvider newValue) newValue.TimeChanged += DataProviderOnTimeChanged;
        }

        private void DataProviderOnTimeChanged(object sender, uint time)
        {
            Dispatcher.InvokeAsync(() =>
            {
                FrameSlider.Value = time;
                CurrentFrameTextBox.Text = time.ToString();
            });
        }

        private void CurrentFrameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (uint.TryParse(CurrentFrameTextBox.Text, out uint time) && DataProvider?.Time != time)
            {
                DataProvider?.MoveToFrame((uint)Math.Min(time, DataProvider.FrameMaximumKey));
            }
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
