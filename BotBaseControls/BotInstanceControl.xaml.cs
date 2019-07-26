﻿using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using BotBase;
using BotBase.Annotations;
using BotBase.BotInstance;
using BotBase.Interfaces;

namespace BotBaseControls
{
    /// <summary>
    /// Interaction logic for BotInstanceControl.xaml
    /// </summary>
    public partial class BotInstanceControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty BotInstanceProperty = DependencyProperty.Register(
            "BotInstance", typeof(BotInstance), typeof(BotInstanceControl), new PropertyMetadata(default(BotInstance)));

        public BotInstance BotInstance
        {
            get => (BotInstance)GetValue(BotInstanceProperty);
            set => SetValue(BotInstanceProperty, value);
        }

        public Type[] DataProviderTypes { get; private set; }
        public Type[] DataLoggerTypes { get; private set; }
        public Type[] SolverTypes { get; private set; }

        public IDataProvider DataProvider => BotInstance?.DataProvider;
        public IDataLogger DataLogger => BotInstance?.DataLogger;
        public ISolver Solver => BotInstance?.Solver;

        public BotInstanceControl()
        {
            InitializeComponent();
        }

        public BotInstanceControl(BotInstance botInstance) : this()
        {
            BotInstance = botInstance;
        }

        private void BotInstance_OnLoaded(object sender, RoutedEventArgs e)
        {
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            //------------------------------------------------------------------------------------------------------------------
            var dataProviderTypes = PluginLoader.LoadPlugins(path, typeof(IDataProvider)).ToArray();

            DataProviderTypes = dataProviderTypes;
            OnPropertyChanged(nameof(DataProviderTypes));

            if (DataProvider != null)
                DataProviderComboBox.SelectedIndex = Array.IndexOf(DataProviderTypes, DataProvider.GetType());

            DataProviderComboBox.SelectionChanged += DataProviderComboBox_OnSelectionChanged;

            //------------------------------------------------------------------------------------------------------------------
            var dataLoggerTypes = PluginLoader.LoadPlugins(path, typeof(IDataLogger)).ToArray();

            DataLoggerTypes = dataLoggerTypes;
            OnPropertyChanged(nameof(DataLoggerTypes));

            if (DataLogger != null)
                DataLoggerComboBox.SelectedIndex = Array.IndexOf(DataLoggerTypes, DataLogger.GetType());

            DataLoggerComboBox.SelectionChanged += DataLoggerComboBoxOnSelectionChanged;

            //------------------------------------------------------------------------------------------------------------------
            var solverTypes = PluginLoader.LoadPlugins(path, typeof(ISolver)).ToArray();

            SolverTypes = solverTypes;
            OnPropertyChanged(nameof(SolverTypes));

            if (Solver != null)
                SolverComboBox.SelectedIndex = Array.IndexOf(SolverTypes, Solver.GetType());

            SolverComboBox.SelectionChanged += SolverComboBoxOnSelectionChanged;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            BotInstance.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            BotInstance.Stop();
        }

        private void DataProviderComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && BotInstance != null)
                BotInstance.DataProvider = (IDataProvider)Activator.CreateInstance(newValue);

            OnPropertyChanged(nameof(DataProvider));
        }

        private void DataLoggerComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && BotInstance != null)
                BotInstance.DataLogger = (IDataLogger)Activator.CreateInstance(newValue);

            OnPropertyChanged(nameof(DataLogger));
        }

        private void SolverComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && BotInstance != null)
                BotInstance.Solver = (ISolver)Activator.CreateInstance(newValue);

            OnPropertyChanged(nameof(Solver));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
