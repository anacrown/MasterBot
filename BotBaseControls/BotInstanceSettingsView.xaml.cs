using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using BotBase;
using BotBase.BotInstance;
using BotBase.Interfaces;

namespace BotBaseControls
{
    /// <summary>
    /// Interaction logic for BotInstanceSettingsView.xaml
    /// </summary>
    public partial class BotInstanceSettingsView : UserControl
    {
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            "Settings", typeof(BotInstanceSettings), typeof(BotInstanceSettingsView), new PropertyMetadata(default(BotInstanceSettings)));

        public BotInstanceSettings Settings
        {
            get => (BotInstanceSettings) GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public static readonly DependencyProperty DataProviderSettingsTypesProperty = DependencyProperty.Register(
            "DataProviderSettingsTypes", typeof(ObservableCollection<Type>), typeof(BotInstanceSettingsView), new PropertyMetadata(default(ObservableCollection<Type>)));

        public ObservableCollection<Type> DataProviderSettingsTypes
        {
            get => (ObservableCollection<Type>)GetValue(DataProviderSettingsTypesProperty);
            set => SetValue(DataProviderSettingsTypesProperty, value);
        }

        public static readonly DependencyProperty DataLoggerSettingsTypesProperty = DependencyProperty.Register(
            "DataLoggerSettingsTypes", typeof(ObservableCollection<Type>), typeof(BotInstanceSettingsView), new PropertyMetadata(default(ObservableCollection<Type>)));

        public ObservableCollection<Type> DataLoggerSettingsTypes
        {
            get => (ObservableCollection<Type>)GetValue(DataLoggerSettingsTypesProperty);
            set => SetValue(DataLoggerSettingsTypesProperty, value);
        }

        public static readonly DependencyProperty SolverTypesProperty = DependencyProperty.Register(
            "SolverTypes", typeof(ObservableCollection<Type>), typeof(BotInstanceSettingsView), new PropertyMetadata(default(ObservableCollection<Type>)));

        public ObservableCollection<Type> SolverTypes
        {
            get => (ObservableCollection<Type>)GetValue(SolverTypesProperty);
            set => SetValue(SolverTypesProperty, value);
        }

        public BotInstanceSettingsView()
        {
            InitializeComponent();
        }

        private void BotInstanceSettingsView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

            //------------------------------------------------------------------------------------------------------------------
            var dataProviderSettingsTypes = PluginLoader.LoadPlugins(path, typeof(DataProviderSettingsBase)).ToArray();

            DataProviderSettingsTypes = new ObservableCollection<Type>(dataProviderSettingsTypes);

            if (DataProviderSettingsTypes.Count == 1)
                Settings.DataProviderSettings = (DataProviderSettingsBase)Activator.CreateInstance(DataProviderSettingsTypes.First());

            if (Settings.DataProviderSettings != null)
                DataProviderSettingsComboBox.SelectedIndex = DataProviderSettingsTypes.IndexOf(Settings.DataProviderSettings.GetType());

            DataProviderSettingsComboBox.SelectionChanged += DataProviderComboBox_OnSelectionChanged;

            //------------------------------------------------------------------------------------------------------------------
            var dataLoggerSettingsTypes = PluginLoader.LoadPlugins(path, typeof(DataLoggerSettingsBase)).ToArray();

            DataLoggerSettingsTypes = new ObservableCollection<Type>(dataLoggerTypes);

            if (DataLoggerTypes.Count == 1)
                BotInstance.DataLogger = (IDataLogger)Activator.CreateInstance(dataLoggerTypes.First());

            if (BotInstance.DataLogger != null)
                DataLoggerComboBox.SelectedIndex = DataLoggerTypes.IndexOf(BotInstance.DataLogger.GetType());

            DataLoggerComboBox.SelectionChanged += DataLoggerComboBoxOnSelectionChanged;

            //------------------------------------------------------------------------------------------------------------------
//            var solverTypes = PluginLoader.LoadPlugins(path, typeof(ISolver)).ToArray();
//
//            SolverTypes = new ObservableCollection<Type>(solverTypes);
//
//            if (SolverTypes.Count == 1)
//                BotInstance.Solver = (ISolver)Activator.CreateInstance(SolverTypes.First());
//
//            if (BotInstance.Solver != null)
//                SolverComboBox.SelectedIndex = SolverTypes.IndexOf(BotInstance.Solver.GetType());
//
//            SolverComboBox.SelectionChanged += SolverComboBoxOnSelectionChanged;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //BotInstance.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            //BotInstance.Stop();
        }

        private void DataProviderComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && Settings != null)
                Settings.DataProviderSettings = (DataProviderSettingsBase)Activator.CreateInstance(newValue);
        }

        private void DataLoggerComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
//            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;
//
//            if (newValue != null && BotInstance != null)
//                BotInstance.DataLogger = (IDataLogger)Activator.CreateInstance(newValue);
        }

        private void SolverComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
//            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;
//
//            if (newValue != null && BotInstance != null)
//                BotInstance.Solver = (ISolver)Activator.CreateInstance(newValue);
        }
    }
}
