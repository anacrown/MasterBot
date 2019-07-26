using System;
using System.Windows;
using BotBase.BotInstance;
using BotBase.Interfaces;

namespace BotBaseControls
{

    public partial class BotInstanceDebugControl
    {
        public static readonly DependencyProperty BotInstanceProperty = DependencyProperty.Register(
            "BotInstance", typeof(BotInstance), typeof(BotInstanceDebugControl), new PropertyMetadata(default(BotInstance)));

        public BotInstance BotInstance
        {
            get => (BotInstance)GetValue(BotInstanceProperty);
            set => SetValue(BotInstanceProperty, value);
        }

        public BotInstanceDebugControl()
        {
            InitializeComponent();
        }

        public BotInstanceDebugControl(BotInstance botInstance) : this()
        {
            BotInstance = botInstance;
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

        private void BotInstanceOnLogDataReceived(object sender, LogRecord logRecord)
        {
            Dispatcher.InvokeAsync(() =>
            {
//                var logSourceFilter = LogFilterEntries.FirstOrDefault(t => t.Header == sender?.GetType().Name);
//
//                if (logSourceFilter == null)
//                {
//                    LogFilterEntries.Add(new LogFilterEntry { Header = sender.GetType().Name, IsEnabled = true });
//                }

                LogTextBlock.AppendText($"[{sender.GetType().Name}][{logRecord.DataFrame?.Time}] {logRecord.Message}{Environment.NewLine}");
                LogTextBlock.ScrollToEnd();

            });
        }
    }
}
