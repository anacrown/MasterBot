using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BomberMan_SuperAI;
using CodenjoyBot.DataProvider;

namespace Debugger.Controls
{
    public partial class BotInstanceList : UserControl
    {
        public static readonly DependencyProperty InstanceModelsProperty = DependencyProperty.Register(
            "InstanceModels", typeof(ObservableCollection<CodenjoyBot.CodenjoyBotInstance>), typeof(BotInstanceList), new PropertyMetadata(default(ObservableCollection<CodenjoyBot.CodenjoyBotInstance>)));

        public ObservableCollection<CodenjoyBot.CodenjoyBotInstance> InstanceModels
        {
            get => (ObservableCollection<CodenjoyBot.CodenjoyBotInstance>)GetValue(InstanceModelsProperty);
            set => SetValue(InstanceModelsProperty, value);
        }

        public BotInstanceList()
        {
            InitializeComponent();
        }

        private void AddBotInstance_OnClick(object sender, RoutedEventArgs e)
        {
            if (InstanceModels == null)
                InstanceModels = new ObservableCollection<CodenjoyBot.CodenjoyBotInstance>();

            InstanceModels.Add(new CodenjoyBot.CodenjoyBotInstance(new WebSocketDataProvider(), new BomberSolver()));
        }

        private void RemoveBotInstance_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItem is CodenjoyBot.CodenjoyBotInstance selectedItem)
            {
                if (selectedItem.IsStarted)
                    selectedItem.Stop();

                InstanceModels.Remove(selectedItem);
            }
        }
    }
}
