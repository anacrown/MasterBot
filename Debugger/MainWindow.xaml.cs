﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using BomberMan_SuperAI.Annotations;
using CodenjoyBot;
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
                _codenjoyBotInstances = value;
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
        }
        private void DefaultSettingsLoad()
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


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
