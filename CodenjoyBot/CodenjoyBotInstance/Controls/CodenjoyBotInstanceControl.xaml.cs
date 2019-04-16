using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CodenjoyBot.Annotations;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.CodenjoyBotInstance.Controls
{
    /// <summary>
    /// Interaction logic for CodenjoyBotInstanceControl.xaml
    /// </summary>
    public partial class CodenjoyBotInstanceControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty CodenjoyBotInstanceProperty = DependencyProperty.Register(
            "CodenjoyBotInstance", typeof(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance), typeof(CodenjoyBotInstanceControl), new PropertyMetadata(default(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance)));

        public CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance CodenjoyBotInstance
        {
            get => (CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance)GetValue(CodenjoyBotInstanceProperty);
            set => SetValue(CodenjoyBotInstanceProperty, value);
        }

        public Type[] DataProviderTypes { get; private set; }
        public Type[] DataLoggerTypes { get; private set; }
        public Type[] SolverTypes { get; private set; }

        public IDataProvider DataProvider => CodenjoyBotInstance?.DataProvider;
        public IDataLogger DataLogger => CodenjoyBotInstance?.DataLogger;
        public ISolver Solver => CodenjoyBotInstance?.Solver;

        public CodenjoyBotInstanceControl()
        {
            InitializeComponent();
        }

        public CodenjoyBotInstanceControl(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance codenjoyBotInstance) : this()
        {
            CodenjoyBotInstance = codenjoyBotInstance;
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
            CodenjoyBotInstance.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            CodenjoyBotInstance.Stop();
        }

        private void DataProviderComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && CodenjoyBotInstance != null)
                CodenjoyBotInstance.DataProvider = (IDataProvider)Activator.CreateInstance(newValue);

            OnPropertyChanged(nameof(DataProvider));
        }

        private void DataLoggerComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && CodenjoyBotInstance != null)
                CodenjoyBotInstance.DataLogger = (IDataLogger)Activator.CreateInstance(newValue);

            OnPropertyChanged(nameof(DataLogger));
        }

        private void SolverComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Type : null;

            if (newValue != null && CodenjoyBotInstance != null)
                CodenjoyBotInstance.Solver = (ISolver)Activator.CreateInstance(newValue);

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
