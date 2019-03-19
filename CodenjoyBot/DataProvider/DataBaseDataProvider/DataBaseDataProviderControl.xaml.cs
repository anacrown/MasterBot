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

namespace CodenjoyBot.DataProvider.DataBaseDataProvider
{
    /// <summary>
    /// Interaction logic for DataBaseDataProviderControl.xaml
    /// </summary>
    public partial class DataBaseDataProviderControl : UserControl
    {

        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(DataBaseDataProvider), typeof(DataBaseDataProviderControl), new PropertyMetadata(default(DataBaseDataProvider)));

        public DataBaseDataProvider DataProvider
        {
            get => (DataBaseDataProvider) GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public DataBaseDataProviderControl()
        {
            InitializeComponent();
        }

        public DataBaseDataProviderControl(DataBaseDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }
    }
}
