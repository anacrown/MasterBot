using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CodenjoyBot.CodenjoyBotInstance.Controls
{
    public partial class BotInstanceList : UserControl
    {
        public static readonly DependencyProperty InstanceModelsProperty = DependencyProperty.Register(
            "InstanceModels", typeof(ObservableCollection<CodenjoyBotInstance>), typeof(BotInstanceList), new PropertyMetadata(default(ObservableCollection<CodenjoyBotInstance>)));

        public ObservableCollection<CodenjoyBotInstance> InstanceModels
        {
            get => (ObservableCollection<CodenjoyBotInstance>)GetValue(InstanceModelsProperty);
            set => SetValue(InstanceModelsProperty, value);
        }

        public BotInstanceList()
        {
            InitializeComponent();
        }

        private void AddBotInstance_OnClick(object sender, RoutedEventArgs e)
        {
            if (InstanceModels == null)
                InstanceModels = new ObservableCollection<CodenjoyBotInstance>();

            InstanceModels.Add(new CodenjoyBotInstance());
        }

        private void RemoveBotInstance_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItem is CodenjoyBotInstance selectedItem)
            {
                if (selectedItem.IsStarted)
                    selectedItem.Stop();

                InstanceModels.Remove(selectedItem);
            }
        }
    }
}
