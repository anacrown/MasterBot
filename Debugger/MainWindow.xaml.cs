using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CodenjoyBot;
using BomberMan_SuperAI;
using BomberMan_SuperAI.Annotations;
using CodenjoyBot.DataProvider;
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
            if (Properties.Settings.Default.BotinstanseModels == null)
                return;

            foreach (var botInstanceModel in Properties.Settings.Default.BotinstanseModels)
            {
                IDataProvider dataProvider;
                switch (botInstanceModel.ProviderType)
                {
                    case DataProviderType.WebSocket:
                        dataProvider = new WebSocketDataProvider(botInstanceModel.IdentityUser);
                        break;
                    case DataProviderType.FileSystem:
                        dataProvider = new FileSystemDataProvider(botInstanceModel.BoardFile);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                CodenjoyBotInstances.Add(new CodenjoyBotInstance(dataProvider, new BomberSolver()));
            }
        }

        private void DefaultSettingsSave()
        {
            var list = new List<BotinstanseModel>();
            foreach (var codenjoyBotInstance in CodenjoyBotInstances)
            {
                switch (codenjoyBotInstance.DataProvider)
                {
                    case WebSocketDataProvider webSocketDataProvider:
                        list.Add(new BotinstanseModel()
                        {
                            ProviderType = DataProviderType.WebSocket,
                            IdentityUser = webSocketDataProvider.IdentityUser
                        });
                        break;
                    case FileSystemDataProvider fileSystemDataProvider:
                        list.Add(new BotinstanseModel()
                        {
                            ProviderType = DataProviderType.FileSystem,
                            BoardFile = fileSystemDataProvider.BoardFile
                        });
                        break;
                }
            }

            Properties.Settings.Default.BotinstanseModels = list.ToArray();

            Properties.Settings.Default.Save();
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
