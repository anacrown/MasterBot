using System.Windows;
using System.Windows.Controls;

namespace CodenjoyBot
{
    /// <summary>
    /// Interaction logic for CodenjoyBotInstanceDebugControl.xaml
    /// </summary>
    public partial class CodenjoyBotInstanceDebugControl : UserControl
    {

        public static readonly DependencyProperty CodenjoyBotInstanceProperty = DependencyProperty.Register(
            "CodenjoyBotInstance", typeof(CodenjoyBotInstance), typeof(CodenjoyBotInstanceDebugControl), new PropertyMetadata(default(CodenjoyBotInstance)));

        public CodenjoyBotInstance CodenjoyBotInstance
        {
            get { return (CodenjoyBotInstance) GetValue(CodenjoyBotInstanceProperty); }
            set { SetValue(CodenjoyBotInstanceProperty, value); }
        }

        public CodenjoyBotInstanceDebugControl()
        {
            InitializeComponent();
        }

        public CodenjoyBotInstanceDebugControl(CodenjoyBotInstance codenjoyBotInstance) : this()
        {
            CodenjoyBotInstance = codenjoyBotInstance;
        }
    }
}
