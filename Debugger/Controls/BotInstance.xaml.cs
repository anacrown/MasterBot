using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using BomberMan_SuperAI.Annotations;
using CodenjoyBot;
using CodenjoyBot.Interfaces;

namespace Debugger.Controls
{
    public partial class BotInstance : INotifyPropertyChanged
    {
        public static readonly DependencyProperty CodenjoyBotInstanceProperty = DependencyProperty.Register(
            "CodenjoyBotInstance", typeof(CodenjoyBotInstance), typeof(BotInstance), new PropertyMetadata(default(CodenjoyBotInstance)));

        public CodenjoyBotInstance CodenjoyBotInstance
        {
            get => (CodenjoyBotInstance)GetValue(CodenjoyBotInstanceProperty);
            set => SetValue(CodenjoyBotInstanceProperty, value);
        }

        public Type[] DataProviderTypes { get; private set; }

        public IDataProvider DataProvider => CodenjoyBotInstance?.DataProvider;

        public bool IsStarted => CodenjoyBotInstance?.IsStarted ?? false;

        public bool StartButtonIsEnabled => !IsStarted;
        public bool StopButtonIsEnabled => IsStarted;

        public BotInstance()
        {
            InitializeComponent();
        }

        private void BotInstance_OnLoaded(object sender, RoutedEventArgs e)
        {
            var baseType = typeof(IDataProvider);
            var dataProviderTypes = baseType.Assembly.ExportedTypes.Where(t => baseType.IsAssignableFrom(t) && t.IsClass).ToArray();
            DataProviderComboBox.SelectedIndex = Array.IndexOf(dataProviderTypes, DataProvider.GetType());
            DataProviderTypes = dataProviderTypes;
            OnPropertyChanged(nameof(DataProviderTypes));

            DataProviderComboBox.SelectionChanged += DataProviderComboBox_OnSelectionChanged;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            CodenjoyBotInstance.Start();
            OnPropertyChanged(nameof(IsStarted));
            OnPropertyChanged(nameof(StartButtonIsEnabled));
            OnPropertyChanged(nameof(StopButtonIsEnabled));
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            CodenjoyBotInstance.Stop();
            OnPropertyChanged(nameof(IsStarted));
            OnPropertyChanged(nameof(StartButtonIsEnabled));
            OnPropertyChanged(nameof(StopButtonIsEnabled));
        }

        private void DataProviderComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && CodenjoyBotInstance != null)
                CodenjoyBotInstance.DataProvider = (IDataProvider)Activator.CreateInstance(newValue);

            OnPropertyChanged(nameof(DataProvider));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
