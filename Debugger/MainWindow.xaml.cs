﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CodenjoyBot;
using CodenjoyBot.Annotations;
using CodenjoyBot.CodenjoyBotInstance;

namespace Debugger
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private CodenjoyBotInstanceCollection _codenjoyBotInstances;
        private CodenjoyBotInstance _selectedBotInstance;
        private SettingsCollection _unVisibleSettings;

        public CodenjoyBotInstanceCollection CodenjoyBotInstances
        {
            get => _codenjoyBotInstances ?? (_codenjoyBotInstances = new CodenjoyBotInstanceCollection());
            private set
            {
                if (Equals(value, _codenjoyBotInstances)) return;

                if (_codenjoyBotInstances != null)
                    _codenjoyBotInstances.Started -= CodenjoyBotInstancesOnStarted;

                _codenjoyBotInstances = value;

                if (_codenjoyBotInstances != null)
                    _codenjoyBotInstances.Started += CodenjoyBotInstancesOnStarted;

                OnPropertyChanged();
            }
        }

        public SettingsCollection UnVisibleSettings
        {
            get => _unVisibleSettings;
            set
            {
                if (Equals(value, _unVisibleSettings)) return;

                if (_unVisibleSettings != null)
                {
                    _unVisibleSettings.DoCommandEvent -= UnVisibleSettingsOnDoCommandEvent;
                }

                _unVisibleSettings = value;

                if (_unVisibleSettings != null)
                {
                    _unVisibleSettings.DoCommandEvent += UnVisibleSettingsOnDoCommandEvent;
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

            using (var db = new CodenjoyDbContext())
            {
                foreach (var botInstance in CodenjoyBotInstances)
                {
                    if (botInstance.SettingsId == null)
                    {
                        var hashCode = botInstance.GetHashCode();
                        var settings = db.LaunchSettingsModels.FirstOrDefault(t => t.HashCode == botInstance.GetHashCode());
                        if (settings == null || settings.HashCode != hashCode)
                        {
                            var newSettings = db.LaunchSettingsModels.FirstOrDefault(t => t.HashCode == hashCode);
                            if (newSettings == null)
                            {
                                if (settings != null) settings.Visibility = false;
                                newSettings = CodenjoyBotInstance.GetSettings(botInstance);
                                db.LaunchSettingsModels.Add(newSettings);
                                newSettings.Visibility = true;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            settings.Visibility = true;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var settings = db.LaunchSettingsModels.Find(botInstance.SettingsId);
                        if (settings == null || settings.HashCode != botInstance.GetHashCode())
                        {
                            var newSettings = db.LaunchSettingsModels.FirstOrDefault(t => t.HashCode == botInstance.GetHashCode());
                            if (newSettings != null)
                            {
                                if (settings != null) settings.Visibility = false;
                                newSettings.Visibility = true;
                                db.SaveChanges();
                            }
                            else throw new Exception("Settings not found");
                        }
                        else
                        {
                            settings.Visibility = true;
                            db.SaveChanges();
                        }
                    }
                }
            }
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
                using (var db = new CodenjoyDbContext())
                {
                    CodenjoyBotInstances = new CodenjoyBotInstanceCollection(
                        db.LaunchSettingsModels
                        .Where(settings => settings.Visibility)
                        .Select(settings => CodenjoyBotInstance.FromSettings(settings)));

                    UnVisibleSettings = new SettingsCollection(db.LaunchSettingsModels
                        .Where(settings => !settings.Visibility)
                        .Select(settings => new SettingsViewModel(settings)).ToArray());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void BattleBotInstanceComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as CodenjoyBotInstance : null;
            var oldValue = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as CodenjoyBotInstance : null;

            SelectedBotInstance = newValue;
        }
        private void UnVisibleSettingsOnDoCommandEvent(object sender, SettingsViewModel viewModel)
        {
            using (var db = new CodenjoyDbContext())
            {
                var settings = db.LaunchSettingsModels.Find(viewModel.Id);
                if (settings == null)
                    throw new Exception("Settings not found");

                CodenjoyBotInstances.Add(CodenjoyBotInstance.FromSettings(settings));

                settings.Visibility = true;

                db.SaveChanges();
            }

            UnVisibleSettings.Remove(viewModel);
        }
        private void BotInstanceList_OnRemoveInstance(object sender, CodenjoyBotInstance botInstance)
        {
            if (botInstance.IsStarted)
                botInstance.Stop();

            using (var db = new CodenjoyDbContext())
            {
                var settings = db.LaunchSettingsModels.Find(botInstance.SettingsId);
                if (settings == null)
                    throw new Exception("Settings not found");

                UnVisibleSettings.Add(new SettingsViewModel(settings));

                settings.Visibility = false;

                db.SaveChanges();
            }
        }
        private void CodenjoyBotInstancesOnStarted(object sender, CodenjoyBotInstance botInstance)
        {
            var settings = UnVisibleSettings.FirstOrDefault(t => t.Id == botInstance.SettingsId);
            if (settings != null)
                Dispatcher.Invoke(() => UnVisibleSettings.Remove(settings));

            Dispatcher.Invoke(() => TabControl.SelectedIndex = 1);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
