using System.Windows;
using System.Windows.Controls;

namespace WebSocketDataProviderView
{
    /// <summary>
    /// Логика взаимодействия для WebSocketDataProviderView.xaml
    /// </summary>
    public partial class WebSocketDataProviderView : UserControl
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(WebSocketDataProvider.WebSocketDataProvider), typeof(WebSocketDataProviderView), new PropertyMetadata(default(WebSocketDataProvider.WebSocketDataProvider)));

        public WebSocketDataProvider.WebSocketDataProvider DataProvider
        {
            get => (WebSocketDataProvider.WebSocketDataProvider) GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }
        public WebSocketDataProviderView()
        {
            InitializeComponent();
        }
    }
}
