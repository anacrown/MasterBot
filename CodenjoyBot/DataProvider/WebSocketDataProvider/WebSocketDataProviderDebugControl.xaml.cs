using System.Windows;
using System.Windows.Controls;

namespace CodenjoyBot.DataProvider.WebSocketDataProvider
{
    /// <summary>
    /// Interaction logic for WebSocketDataProviderDebugControl.xaml
    /// </summary>
    public partial class WebSocketDataProviderDebugControl : UserControl
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(WebSocketDataProvider), typeof(WebSocketDataProviderDebugControl), new PropertyMetadata(default(WebSocketDataProvider)));

        public WebSocketDataProvider DataProvider
        {
            get => (WebSocketDataProvider) GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public WebSocketDataProviderDebugControl()
        {
            InitializeComponent();
        }

        public WebSocketDataProviderDebugControl(WebSocketDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }
    }
}
