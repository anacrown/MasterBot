using System;
using System.Windows;
using System.Windows.Controls;

namespace CodenjoyBot.DataProvider.DataBaseDataProvider
{
    /// <summary>
    /// Interaction logic for DataBaseDataProviderDebugControl.xaml
    /// </summary>
    public partial class DataBaseDataProviderDebugControl : UserControl
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(DataBaseDataProvider), typeof(DataBaseDataProviderDebugControl), new PropertyMetadata(default(DataBaseDataProvider)));

        public DataBaseDataProvider DataProvider
        {
            get => (DataBaseDataProvider)GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public DataBaseDataProviderDebugControl()
        {
            InitializeComponent();
        }

        public DataBaseDataProviderDebugControl(DataBaseDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DataProviderProperty)
            {
                if (e.OldValue is DataBaseDataProvider oldDataProvider)
                {
                    oldDataProvider.Started -= DataProviderOnStarted;
                    oldDataProvider.Stopped -= DataProviderOnStopped;
                    oldDataProvider.TimeChanged -= DataProviderOnTimeChanged;
                }

                if (e.NewValue is DataBaseDataProvider newDataProvider)
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
                DataProvider?.MoveToFrame(Math.Min(time, DataProvider.FrameMaximumKey));
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
            var time = (uint) e.NewValue;

            if (DataProvider.Time != time)
                DataProvider.MoveToFrame(time);
        }
    }
}
