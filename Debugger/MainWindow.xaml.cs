using System.Windows;

using CodenjoyBot;
using BomberMan_SuperAI;
using CodenjoyBot.DataProvider;

namespace Debugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var botInstance = new CodenjoyBotInstance(new WebSocketDataProvider(new IdentityUser("ws://codenjoy.com/codenjoy-contest/ws", "j99lpu1l8skamhdzbyq9", "7040034271572867319")), new BomberSolver());
            botInstance.DataProvider.Start();
        }
    }
}
