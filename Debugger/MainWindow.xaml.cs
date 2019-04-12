using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using BomberMan_SuperAI.Annotations;
using CodenjoyBot.CodenjoyBotInstance;
using System.Runtime.Serialization.Formatters.Soap;

namespace Debugger
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private CodenjoyBotCollection _codenjoyBotInstances;
        private CodenjoyBotInstance _selectedBotInstance;

        public CodenjoyBotCollection CodenjoyBotInstances
        {
            get => _codenjoyBotInstances ?? (_codenjoyBotInstances = new CodenjoyBotCollection());
            private set
            {
                if (Equals(value, _codenjoyBotInstances)) return;

                if (_codenjoyBotInstances != null)
                {
                    _codenjoyBotInstances.Started -= CodenjoyBotInstancesOnStarted;
                    _codenjoyBotInstances.Stopped -= CodenjoyBotInstancesOnStopped;
                }

                _codenjoyBotInstances = value;

                if (_codenjoyBotInstances != null)
                {
                    _codenjoyBotInstances.Started += CodenjoyBotInstancesOnStarted;
                    _codenjoyBotInstances.Stopped += CodenjoyBotInstancesOnStopped;
                }

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
            AppData.Set();
             
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
            var settingsFile = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
                "Settings.bin");

            if (!File.Exists(settingsFile))
                return;

            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(settingsFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var instances = (CodenjoyBotInstance[])formatter.Deserialize(stream);
                    CodenjoyBotInstances = new CodenjoyBotCollection(instances);
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                if (MessageBox.Show(this, $"{e}\r\n\r\nУдалить файл настроек?", "Exception", MessageBoxButton.YesNo, MessageBoxImage.Error,
                        MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    File.Delete(settingsFile);
                }
            }
        }

        private void DefaultSettingsSave()
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Settings.bin"), FileMode.Create, FileAccess.Write, FileShare.None))
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

        private void CodenjoyBotInstancesOnStarted(object sender, CodenjoyBotInstance e)
        {
            MessageBox.Show($"{e.Title} Started", "Information");
        }

        private void CodenjoyBotInstancesOnStopped(object sender, CodenjoyBotInstance e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
