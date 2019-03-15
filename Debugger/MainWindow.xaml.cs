using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using BomberMan_SuperAI;
using CodenjoyBot;
using BomberMan_SuperAI.Annotations;
using CodenjoyBot.DataProvider.FileSystemDataProvider;
using CodenjoyBot.DataProvider.WebSocketDataProvider;
using CodenjoyBot.Interfaces;

namespace Debugger
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private ObservableCollection<CodenjoyBotInstance> _codenjoyBotInstances;
        private CodenjoyBotInstance _selectedBotInstance;

        public ObservableCollection<CodenjoyBotInstance> CodenjoyBotInstances
        {
            get => _codenjoyBotInstances ?? (_codenjoyBotInstances = new ObservableCollection<CodenjoyBotInstance>());
            private set
            {
                if (Equals(value, _codenjoyBotInstances)) return;
                _codenjoyBotInstances = value;
                OnPropertyChanged();
            }
        }

        public CodenjoyBotInstance SelectedBotInstance
        {
            get => _selectedBotInstance;
            set
            {
                _selectedBotInstance = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DefaultSettingsLoad();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            foreach (var codenjoyBotInstance in CodenjoyBotInstances)
                codenjoyBotInstance?.Stop();

            DefaultSettingsSave();
        }

        private void DefaultSettingsLoad()
        {
            if (!File.Exists("Settings.bin"))
                return;

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream("Settings.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var instances = (CodenjoyBotInstance[])formatter.Deserialize(stream);
                CodenjoyBotInstances = new ObservableCollection<CodenjoyBotInstance>(instances);
                stream.Close();
            }
        }

        private void DefaultSettingsSave()
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream("Settings.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, CodenjoyBotInstances.ToArray());
                stream.Close();
            }
        }

        private void BattleBotInstanceComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as CodenjoyBotInstance : null;
            var oldValue = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as CodenjoyBotInstance : null;

            SelectedBotInstance = newValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
