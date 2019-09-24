using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
