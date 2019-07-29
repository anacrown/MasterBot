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
using BotBase;

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
            get => (BotInstance) GetValue(BotInstanceProperty);
            set => SetValue(BotInstanceProperty, value);
        }

        public BotInstanceView()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BotInstanceProperty)
            {
                if (e.OldValue is BotInstance oldValue)
                    oldValue.LogDataReceived -= BotInstanceOnLogDataReceived;

                if (e.NewValue is BotInstance newValue)
                    newValue.LogDataReceived += BotInstanceOnLogDataReceived;
            }
        }

        private void BotInstanceOnLogDataReceived(object sender, LogRecord e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                //                var logSourceFilter = LogFilterEntries.FirstOrDefault(t => t.Header == sender?.GetType().Name);
                //
                //                if (logSourceFilter == null)
                //                {
                //                    LogFilterEntries.Add(new LogFilterEntry { Header = sender.GetType().Name, IsEnabled = true });
                //                }

                LogTextBlock.AppendText($"[{sender.GetType().Name}][{e.DataFrame?.FrameNumber}] {e.Message}{Environment.NewLine}");
                LogTextBlock.ScrollToEnd();

            });
        }
    }
}
