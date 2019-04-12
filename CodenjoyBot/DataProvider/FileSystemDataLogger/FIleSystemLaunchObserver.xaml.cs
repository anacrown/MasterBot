using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CodenjoyBot.DataProvider.FileSystemDataLogger
{
    /// <summary>
    /// Логика взаимодействия для FIleSystemLaunchObserver.xaml
    /// </summary>
    public partial class FIleSystemLaunchObserver : UserControl
    {
        public static readonly DependencyProperty LaunchNodesProperty = DependencyProperty.Register(
            "LaunchNodes", typeof(FileSystemLaunchNode[]), typeof(FIleSystemLaunchObserver), new PropertyMetadata(default(FileSystemLaunchNode[])));

        public FileSystemLaunchNode[] LaunchNodes
        {
            get { return (FileSystemLaunchNode[]) GetValue(LaunchNodesProperty); }
            set { SetValue(LaunchNodesProperty, value); }
        }

        public FIleSystemLaunchObserver()
        {
            InitializeComponent();
        }

        public void Load()
        {
            LaunchNodes = FileSystemDataLogger.GetLaunches().ToLookup(launch => launch.BotInstanceName).Select(lookup => new FileSystemLaunchNode()
            {
                Name = lookup.Key,
                Launches = lookup.ToArray()
            }).ToArray();
        }
    }

    public class FileSystemLaunchNode
    {
        public string Name { get; set; }
        public FileSystemLaunchInfo[] Launches { get; set; }
    }
}
