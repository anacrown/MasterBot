using System;
using System.Windows;
using System.Windows.Controls;

namespace DataBaseDataProviderView
{
    /// <summary>
    /// Interaction logic for DataBaseDataProviderDebugControl.xaml
    /// </summary>
    public partial class DataBaseDataProviderDebugControl : UserControl
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(DataBaseDataProvider.DataBaseDataProvider), typeof(DataBaseDataProviderDebugControl), new PropertyMetadata(default(DataBaseDataProvider.DataBaseDataProvider)));

        public DataBaseDataProvider.DataBaseDataProvider DataProvider
        {
            get => (DataBaseDataProvider.DataBaseDataProvider)GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public DataBaseDataProviderDebugControl()
        {
            InitializeComponent();
        }

        public DataBaseDataProviderDebugControl(DataBaseDataProvider.DataBaseDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DataProviderProperty)
            {
                if (e.OldValue is DataBaseDataProvider.DataBaseDataProvider oldDataProvider)
                {
                    oldDataProvider.Started -= DataProviderOnStarted;
                    oldDataProvider.Stopped -= DataProviderOnStopped;
                    oldDataProvider.TimeChanged -= DataProviderOnTimeChanged;
                }

                if (e.NewValue is DataBaseDataProvider.DataBaseDataProvider newDataProvider)
                {
                    newDataProvider.Started -= DataProviderOnStarted;
                    newDataProvider.Stopped -= DataProviderOnStopped;
                    newDataProvider.TimeChanged += DataProviderOnTimeChanged;
                }
            }
        }

        private void DataProviderOnStarted(object sender, EventArgs e)
        {

        }

        private void DataProviderOnStopped(object sender, EventArgs e)
        {

        }

        private void DataProviderOnTimeChanged(object sender, uint time)
        {
            Dispatcher?.InvokeAsync(() =>
            {
                FrameSlider.Value = time;
                CurrentFrameTextBox.Text = time.ToString();
            });
        }

        private void CurrentFrameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (uint.TryParse(CurrentFrameTextBox.Text, out var frameNumber) && DataProvider?.FrameNumber != frameNumber)
            {
                DataProvider?.MoveToFrame(Math.Min(frameNumber, DataProvider.FrameMaximumKey));
            }
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            DataProvider?.RecordPlay();

            PlayButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            DataProvider?.RecordStop();

            StopButton.IsEnabled = false;
            PlayButton.IsEnabled = true;
        }

        private void FrameSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var frameNumber = (uint) e.NewValue;

            if (DataProvider.FrameNumber != frameNumber)
                DataProvider.MoveToFrame(frameNumber);
        }
    }
}
