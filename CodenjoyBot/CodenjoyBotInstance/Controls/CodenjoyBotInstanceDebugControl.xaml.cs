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

    public partial class CodenjoyBotInstanceDebugControl
    {
        public static readonly DependencyProperty CodenjoyBotInstanceProperty = DependencyProperty.Register(
            "CodenjoyBotInstance", typeof(CodenjoyBotInstance), typeof(CodenjoyBotInstanceDebugControl), new PropertyMetadata(default(CodenjoyBotInstance)));

        public CodenjoyBotInstance CodenjoyBotInstance
        {
            get => (CodenjoyBotInstance)GetValue(CodenjoyBotInstanceProperty);
            set => SetValue(CodenjoyBotInstanceProperty, value);
        }

        public CodenjoyBotInstanceDebugControl()
        {
            InitializeComponent();
        }

        public CodenjoyBotInstanceDebugControl(CodenjoyBotInstance codenjoyBotInstance) : this()
        {
            CodenjoyBotInstance = codenjoyBotInstance;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == CodenjoyBotInstanceProperty)
            {
                if (e.OldValue is CodenjoyBotInstance oldValue)
                    oldValue.LogDataReceived -= CodenjoyBotInstanceOnLogDataReceived;

                if (e.NewValue is CodenjoyBotInstance newValue)
                    newValue.LogDataReceived += CodenjoyBotInstanceOnLogDataReceived;
            }
        }

        private void CodenjoyBotInstanceOnLogDataReceived(object sender, LogRecord logRecord)
        {
            Dispatcher.InvokeAsync(() =>
            {
                var logSourceFilter = CodenjoyBotInstance.LogFilterEntries.FirstOrDefault(t => t.Header == sender?.GetType().Name);

                if (logSourceFilter == null)
                {
                    CodenjoyBotInstance.LogFilterEntries.Add(new LogFilterEntry { Header = sender.GetType().Name, IsEnabled = true });
                }

                LogTextBlock.AppendText(
                        $"[{sender.GetType().Name}][{logRecord.DataFrame?.Time}] {logRecord.Message}{Environment.NewLine}");
                LogTextBlock.ScrollToEnd();

            });
        }
    }
}
