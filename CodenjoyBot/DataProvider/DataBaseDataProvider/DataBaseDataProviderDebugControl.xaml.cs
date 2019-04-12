using System.Windows;
using System.Windows.Controls;

namespace CodenjoyBot.DataProvider.DataBaseDataProvider
{
    /// <summary>
    /// Interaction logic for DataBaseDataProviderDebugControl.xaml
    /// </summary>
    public partial class DataBaseDataProviderDebugControl : UserControl
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(DataBaseDataProvider), typeof(DataBaseDataProviderDebugControl), new PropertyMetadata(default(DataBaseDataProvider)));

        public DataBaseDataProvider DataProvider
        {
            get => (DataBaseDataProvider) GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public DataBaseDataProviderDebugControl()
        {
            InitializeComponent();
        }

        public DataBaseDataProviderDebugControl(DataBaseDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }
    }
}
