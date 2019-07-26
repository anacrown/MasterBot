using System.Windows;
using System.Windows.Controls;

namespace WebSocketDataProviderView
{
    /// <summary>
    /// Interaction logic for WebSocketDataProviderDebugControl.xaml
    /// </summary>
    public partial class WebSocketDataProviderDebugControl : UserControl
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(WebSocketDataProvider.WebSocketDataProvider), typeof(WebSocketDataProviderDebugControl), new PropertyMetadata(default(WebSocketDataProvider.WebSocketDataProvider)));

        public WebSocketDataProvider.WebSocketDataProvider DataProvider
        {
            get => (WebSocketDataProvider.WebSocketDataProvider) GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public WebSocketDataProviderDebugControl()
        {
            InitializeComponent();
        }

        public WebSocketDataProviderDebugControl(WebSocketDataProvider.WebSocketDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }
    }
}
