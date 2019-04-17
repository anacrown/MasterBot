using System.Windows;
using System.Windows.Controls;

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
            get => (DataBaseDataProvider)GetValue(DataProviderProperty);
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

        private void SelectInstance_OnClick(object sender, RoutedEventArgs e)
        {
            var selectInstanceWindow = new SelectInstanceWindow {Owner = Application.Current.MainWindow};
            SessionTitle.Text = selectInstanceWindow.ShowDialog() == true ? selectInstanceWindow.SelectedLaunch.Header : string.Empty;
        }
    }
}
