using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using BotBase;

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
            get => (BotInstanceSettings)GetValue(SettingsProperty);
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

        public static readonly DependencyProperty SolverSettingsTypesProperty = DependencyProperty.Register(
            "SolverSettingsTypes", typeof(ObservableCollection<Type>), typeof(BotInstanceSettingsView), new PropertyMetadata(default(ObservableCollection<Type>)));

        public ObservableCollection<Type> SolverSettingsTypes
        {
            get => (ObservableCollection<Type>)GetValue(SolverSettingsTypesProperty);
            set => SetValue(SolverSettingsTypesProperty, value);
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

            if (Settings.DataProviderSettings == null && DataProviderSettingsTypes.Count == 1)
                Settings.DataProviderSettings = (DataProviderSettingsBase)Activator.CreateInstance(DataProviderSettingsTypes.First());

            if (Settings.DataProviderSettings != null)
                DataProviderSettingsComboBox.SelectedIndex = DataProviderSettingsTypes.IndexOf(Settings.DataProviderSettings.GetType());

            DataProviderSettingsComboBox.SelectionChanged += DataProviderSettingsComboBox_OnSelectionChanged;

            //------------------------------------------------------------------------------------------------------------------
            var dataLoggerSettingsTypes = PluginLoader.LoadPlugins(path, typeof(DataLoggerSettingsBase)).ToArray();

            DataLoggerSettingsTypes = new ObservableCollection<Type>(dataLoggerSettingsTypes);

            if (Settings.DataLoggerSettings == null && DataLoggerSettingsTypes.Count == 1)
                Settings.DataLoggerSettings = (DataLoggerSettingsBase)Activator.CreateInstance(dataLoggerSettingsTypes.First());

            if (Settings.DataLoggerSettings != null)
                DataLoggerSettingsComboBox.SelectedIndex = DataLoggerSettingsTypes.IndexOf(Settings.DataLoggerSettings.GetType());

            DataLoggerSettingsComboBox.SelectionChanged += DataLoggerSettingsComboBoxOnSelectionChanged;

            //------------------------------------------------------------------------------------------------------------------
            var solverSettingsTypes = PluginLoader.LoadPlugins(path, typeof(SolverSettingsBase)).ToArray();

            SolverSettingsTypes = new ObservableCollection<Type>(solverSettingsTypes);

            if (Settings.SolverSettings == null && SolverSettingsTypes.Count == 1)
                Settings.SolverSettings = (SolverSettingsBase)Activator.CreateInstance(SolverSettingsTypes.First());

            if (Settings.SolverSettings != null)
                SolverSettingsComboBox.SelectedIndex = SolverSettingsTypes.IndexOf(Settings.SolverSettings.GetType());

            SolverSettingsComboBox.SelectionChanged += SolverComboBoxOnSelectionChanged;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //BotInstance.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            //BotInstance.Stop();
        }

        private void DataProviderSettingsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && Settings != null)
                Settings.DataProviderSettings = (DataProviderSettingsBase)Activator.CreateInstance(newValue);
        }

        private void DataLoggerSettingsComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && Settings != null)
                Settings.DataLoggerSettings = (DataLoggerSettingsBase)Activator.CreateInstance(newValue);
        }

        private void SolverComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && Settings != null)
                Settings.SolverSettings = (SolverSettingsBase)Activator.CreateInstance(newValue);
        }
    }
}
