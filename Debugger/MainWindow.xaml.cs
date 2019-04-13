using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using BomberMan_SuperAI.Annotations;
using CodenjoyBot;
using CodenjoyBot.CodenjoyBotInstance;

namespace Debugger
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private CodenjoyBotInstanceCollection _codenjoyBotInstances;
        private CodenjoyBotInstance _selectedBotInstance;

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
