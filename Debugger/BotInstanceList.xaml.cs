using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BattleBot_SuperAI.BattleSolver;
using BomberMan_SuperAI;
using BomberMan_SuperAI.BattleSolver;
using CodenjoyBot.CodenjoyBotInstance;
using CodenjoyBot.DataProvider;
using CodenjoyBot.DataProvider.WebSocketDataProvider;

namespace Debugger
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

        private void AddDebugBotInstance_OnClick(object sender, RoutedEventArgs e)
        {
            InstanceModels.Add(new CodenjoyBotInstance(new WebSocketDataProvider(new IdentityUser("ws://92.124.142.118:8080/codenjoy-contest/ws", "nais@mail.ru", "13476795611535248716")), new BattleSolver()));
        }
    }
}
