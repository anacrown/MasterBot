using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
using BotBase;
using BotBase.Interfaces;

namespace BotBaseControls
{
    /// <summary>
    /// Логика взаимодействия для BotInstanceView.xaml
    /// </summary>
    public partial class BotInstanceView : UserControl
    {
        public static readonly DependencyProperty BotInstanceProperty = DependencyProperty.Register(
            "BotInstance", typeof(BotInstance), typeof(BotInstanceView), new PropertyMetadata(default(BotInstance)));

        public BotInstance BotInstance
        {
            get => (BotInstance)GetValue(BotInstanceProperty);
            set => SetValue(BotInstanceProperty, value);
        }

        public static readonly DependencyProperty FilterRecordsProperty = DependencyProperty.Register(
            "FilterRecords", typeof(ObservableCollection<FilterRecord>), typeof(BotInstanceView), new PropertyMetadata(default(ObservableCollection<FilterRecord>)));

        public ObservableCollection<FilterRecord> FilterRecords
        {
            get => (ObservableCollection<FilterRecord>)GetValue(FilterRecordsProperty);
            set => SetValue(FilterRecordsProperty, value);
        }

        private uint _lastTick = 0;

        public BotInstanceView()
        {
            InitializeComponent();

            ClearBeforeTickCheckBox.IsChecked = Properties.Settings.Default.ClearLogBeforeTick;

            FilterRecords = new ObservableCollection<FilterRecord>();
            if (Properties.Settings.Default.DisabledFilterRecords != null)
            {
                foreach (var recordHeader in Properties.Settings.Default.DisabledFilterRecords)
                {
                    var filterRecord = new FilterRecord() { Header = recordHeader, IsEnabled = false };
                    filterRecord.PropertyChanged += FilterRecordOnPropertyChanged;
                    FilterRecords.Add(filterRecord);
                }
            }

        }

        private void FilterRecordOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Properties.Settings.Default.DisabledFilterRecords != null)
                Properties.Settings.Default.DisabledFilterRecords.Clear();
            else Properties.Settings.Default.DisabledFilterRecords = new StringCollection();

            Properties.Settings.Default.DisabledFilterRecords.AddRange(FilterRecords.Where(t => !t.IsEnabled).Select(t => t.Header).ToArray());
            Properties.Settings.Default.Save();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BotInstanceProperty)
            {
                if (e.OldValue is BotInstance oldValue)
                {
                    oldValue.Started -= BotInstanceOnStarted;
                    oldValue.LogDataReceived -= BotInstanceOnLogDataReceived;
                }

                if (e.NewValue is BotInstance newValue)
                {
                    newValue.Started += BotInstanceOnStarted;
                    newValue.LogDataReceived += BotInstanceOnLogDataReceived;
                }
            }
        }

        private void BotInstanceOnStarted(object sender, IDataProvider e)
        {
            if (VisualTreeHelper.GetChild(DataProviderPresenter, 0) is UIElement container)
            {
                container.Focus();
            }
        }

        private void BotInstanceOnLogDataReceived(object sender, LogRecord e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (_lastTick != e.DataFrame.FrameNumber)
                {
                    LogTextBlock.Clear();
                    _lastTick = e.DataFrame.FrameNumber;
                }

                var filterRecord = FilterRecords.FirstOrDefault(t => t.Header == sender?.GetType().Name);

                if (filterRecord == null)
                {
                    FilterRecords.Add(filterRecord = new FilterRecord { Header = sender.GetType().Name, IsEnabled = true });
                    filterRecord.PropertyChanged += FilterRecordOnPropertyChanged;
                }

                if (filterRecord.IsEnabled)
                {
                    LogTextBlock.AppendText($"[{sender.GetType().Name}][{e.DataFrame.FrameNumber}] {e.Message}{Environment.NewLine}");
                    LogTextBlock.ScrollToEnd();
                }
            });
        }

        private void ClearLogButton_OnClick(object sender, RoutedEventArgs e)
        {
            LogTextBlock.Clear();
        }

        private void ClearBeforeTickCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ClearLogBeforeTick = ClearBeforeTickCheckBox.IsChecked == true;
            Properties.Settings.Default.Save();
        }
    }
}
