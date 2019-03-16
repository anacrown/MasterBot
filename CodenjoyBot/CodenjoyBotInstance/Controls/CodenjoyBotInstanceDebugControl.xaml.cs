using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Windows;
using CodenjoyBot.Annotations;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.CodenjoyBotInstance.Controls
{

    public partial class CodenjoyBotInstanceDebugControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty CodenjoyBotInstanceProperty = DependencyProperty.Register(
            "CodenjoyBotInstance", typeof(CodenjoyBotInstance), typeof(CodenjoyBotInstanceDebugControl), new PropertyMetadata(default(CodenjoyBotInstance)));

        public CodenjoyBotInstance CodenjoyBotInstance
        {
            get => (CodenjoyBotInstance)GetValue(CodenjoyBotInstanceProperty);
            set => SetValue(CodenjoyBotInstanceProperty, value);
        }

        public ObservableCollection<LogFilterEntry> LogFilters { get; } =
            new ObservableCollection<LogFilterEntry>();

        public CodenjoyBotInstanceDebugControl()
        {
            InitializeComponent();
        }

        public CodenjoyBotInstanceDebugControl(CodenjoyBotInstance codenjoyBotInstance, LogFilterEntry[] logFilterEntries = null) : this()
        {
            CodenjoyBotInstance = codenjoyBotInstance;

            if (logFilterEntries != null)
                LogFilters = new ObservableCollection<LogFilterEntry>(logFilterEntries);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == CodenjoyBotInstanceProperty)
            {
                var oldValue = e.OldValue as CodenjoyBotInstance;
                if (oldValue != null)
                    oldValue.LogDataReceived -= CodenjoyBotInstanceOnLogDataReceived;

                var newValue = e.NewValue as CodenjoyBotInstance;
                if (newValue != null)
                    newValue.LogDataReceived += CodenjoyBotInstanceOnLogDataReceived;
            }
        }

        private void CodenjoyBotInstanceOnLogDataReceived(object sender, LogRecord logRecord)
        {
            var logSourceFilter = LogFilters.FirstOrDefault(t => t.Header == sender?.GetType().Name);

            if (logSourceFilter == null)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    LogFilters.Add(logSourceFilter = new LogFilterEntry() { Header = sender.GetType().Name, IsEnabled = true });

                    LogTextBlock.AppendText($"[{sender.GetType().Name}][{logRecord.DataFrame?.Time}] {logRecord.Message}{Environment.NewLine}");
                    LogTextBlock.ScrollToEnd();
                });
            }
            else
            {
                if (logSourceFilter.IsEnabled.HasValue && logSourceFilter.IsEnabled.Value)
                    Dispatcher.InvokeAsync(() =>
                    {
                        LogTextBlock.AppendText($"[{sender.GetType().Name}][{logRecord.DataFrame?.Time}] {logRecord.Message}{Environment.NewLine}");
                        LogTextBlock.ScrollToEnd();
                    });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
