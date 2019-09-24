using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace VisioDataProviderView
{
    /// <summary>
    /// Логика взаимодействия для VisioDataProviderView.xaml
    /// </summary>
    public partial class VisioDataProviderView : UserControl
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(VisioDataProvider.VisioDataProvider), typeof(VisioDataProviderView), new PropertyMetadata(default(VisioDataProvider.VisioDataProvider)));

        public VisioDataProvider.VisioDataProvider DataProvider
        {
            get => (VisioDataProvider.VisioDataProvider) GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public static readonly DependencyProperty PlayersProperty = DependencyProperty.Register(
            "Players", typeof(ObservableCollection<string>), typeof(VisioDataProviderView), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> Players
        {
            get => (ObservableCollection<string>) GetValue(PlayersProperty);
            set => SetValue(PlayersProperty, value);
        }

        public VisioDataProviderView()
        {
            InitializeComponent();

            //GotFocus += (sender, args) => FrameSlider.Focus();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property != DataProviderProperty) return;
            if (e.OldValue is VisioDataProvider.VisioDataProvider oldValue)
            {
                oldValue.TimeChanged -= DataProviderOnTimeChanged;
                oldValue.PropertyChanged -= DataProviderOnPropertyChanged;
            }

            if (e.NewValue is VisioDataProvider.VisioDataProvider newValue)
            {
                newValue.TimeChanged += DataProviderOnTimeChanged;
                newValue.PropertyChanged += DataProviderOnPropertyChanged;
            }
        }

        private void DataProviderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataProvider.PlayersCount))
            {
                Dispatcher.InvokeAsync(() =>
                {
                    if (Players == null) Players = new ObservableCollection<string>();
                    for (var i = 1; i <= DataProvider.PlayersCount; i++)
                    {
                        Players.Add($"Player {i}");
                    }

                    PlayerCombobox.SelectedIndex = 0;
                });
            }
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
            if (uint.TryParse(CurrentFrameTextBox.Text, out uint time) && DataProvider?.FrameNumber != time)
            {
                DataProvider?.MoveToFrame(Math.Min(time, (uint)DataProvider.FrameMaximumKey));
            }
        }

        private void FrameSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DataProvider?.MoveToFrame((uint)e.NewValue);
        }

        private void PlayerCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataProvider.CurrentPlayer = PlayerCombobox.SelectedIndex + 1;
        }
    }
}
