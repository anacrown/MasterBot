using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using BotBase;
using BotBase.Annotations;
using BotBase.BotInstance;

namespace Debugger
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private BotInstanceCollection _botInstances;
        private BotInstance _selectedBotInstance;
//        private SettingsCollection _unVisibleSettings;

        public BotInstanceCollection BotInstances
        {
            // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
            get => _botInstances ?? (_botInstances = new BotInstanceCollection());
            private set
            {
                if (Equals(value, _botInstances)) return;
//
//                if (_botInstances != null)
//                    _botInstances.Started -= BotInstancesOnStarted;
//
//                _botInstances = value;
//
//                if (_botInstances != null)
//                    _botInstances.Started += BotInstancesOnStarted;

                OnPropertyChanged();
            }
        }

//        public SettingsCollection UnVisibleSettings
//        {
//            get => _unVisibleSettings;
//            set
//            {
//                if (Equals(value, _unVisibleSettings)) return;
//
//                if (_unVisibleSettings != null)
//                {
//                    _unVisibleSettings.DoCommandEvent -= UnVisibleSettingsOnDoCommandEvent;
//                }
//
//                _unVisibleSettings = value;
//
//                if (_unVisibleSettings != null)
//                {
//                    _unVisibleSettings.DoCommandEvent += UnVisibleSettingsOnDoCommandEvent;
//                }
//
//                OnPropertyChanged();
//            }
//        }

        public BotInstance SelectedBotInstance
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
            foreach (var codenjoyBotInstance in BotInstances)
                codenjoyBotInstance?.Stop();

//            using (var db = new BotDbContext())
//            {
//                foreach (var botInstance in CodenjoyBotInstances)
//                {
//                    if (botInstance.SettingsId == null)
//                    {
//                        var hashCode = botInstance.GetHashCode();
//                        var settings = db.LaunchSettingsModels.FirstOrDefault(t => t.HashCode == botInstance.GetHashCode());
//                        if (settings == null || settings.HashCode != hashCode)
//                        {
//                            var newSettings = db.LaunchSettingsModels.FirstOrDefault(t => t.HashCode == hashCode);
//                            if (newSettings == null)
//                            {
//                                if (settings != null) settings.Visibility = false;
//                                newSettings = BotInstance.GetSettings(botInstance);
//                                db.LaunchSettingsModels.Add(newSettings);
//                                newSettings.Visibility = true;
//                                db.SaveChanges();
//                            }
//                        }
//                        else
//                        {
//                            settings.Visibility = true;
//                            db.SaveChanges();
//                        }
//                    }
//                    else
//                    {
//                        var settings = db.LaunchSettingsModels.Find(botInstance.SettingsId);
//                        if (settings == null || settings.HashCode != botInstance.GetHashCode())
//                        {
//                            var newSettings = db.LaunchSettingsModels.FirstOrDefault(t => t.HashCode == botInstance.GetHashCode());
//                            if (newSettings != null)
//                            {
//                                if (settings != null) settings.Visibility = false;
//                                newSettings.Visibility = true;
//                                db.SaveChanges();
//                            }
//                            else throw new Exception("Settings not found");
//                        }
//                        else
//                        {
//                            settings.Visibility = true;
//                            db.SaveChanges();
//                        }
//                    }
//                }
//            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedBotInstance?.Start();
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedBotInstance?.Stop();
        }

        private void DefaultSettingsLoad()
        {
            try
            {
//                using (var db = new BotDbContext())
//                {
//                    CodenjoyBotInstances = new BotInstanceCollection(
//                        db.LaunchSettingsModels
//                        .Where(settings => settings.Visibility)
//                        .Select(settings => BotInstance.FromSettings(settings)));
//
//                    UnVisibleSettings = new SettingsCollection(db.LaunchSettingsModels
//                        .Where(settings => !settings.Visibility)
//                        .Select(settings => new SettingsViewModel(settings)).ToArray());
//                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void BattleBotInstanceComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as BotInstance : null;
            var oldValue = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as BotInstance : null;

            SelectedBotInstance = newValue;
        }
//        private void UnVisibleSettingsOnDoCommandEvent(object sender, SettingsViewModel viewModel)
//        {
////            using (var db = new CodenjoyDbContext())
////            {
////                var settings = db.LaunchSettingsModels.Find(viewModel.Id);
////                if (settings == null)
////                    throw new Exception("Settings not found");
////
////                CodenjoyBotInstances.Add(CodenjoyBotInstance.FromSettings(settings));
////
////                settings.Visibility = true;
////
////                db.SaveChanges();
////            }
////
////            UnVisibleSettings.Remove(viewModel);
//        }
        private void BotInstanceList_OnRemoveInstance(object sender, BotInstance botInstance)
        {
//            if (botInstance.IsStarted)
//                botInstance.Stop();
//
//            using (var db = new BotDbContext())
//            {
//                var settings = db.LaunchSettingsModels.Find(botInstance.SettingsId);
//                if (settings == null)
//                    throw new Exception("Settings not found");
//
//                UnVisibleSettings.Add(new SettingsViewModel(settings));
//
//                settings.Visibility = false;
//
//                db.SaveChanges();
//            }
        }
//        private void BotInstancesOnStarted(object sender, BotInstance botInstance)
//        {
//            var settings = UnVisibleSettings.FirstOrDefault(t => t.Id == botInstance.SettingsId);
//            if (settings != null)
//                Dispatcher.Invoke(() => UnVisibleSettings.Remove(settings));
//
//            Dispatcher.Invoke(() => TabControl.SelectedIndex = 1);
//        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
