using System.Windows;
using System.Windows.Controls;

namespace DataBaseDataProviderView
{
    /// <summary>
    /// Interaction logic for DataBaseDataProviderControl.xaml
    /// </summary>
    public partial class DataBaseDataProviderControl : UserControl
    {

        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(DataBaseDataProvider.DataBaseDataProvider), typeof(DataBaseDataProviderControl), new PropertyMetadata(default(DataBaseDataProvider.DataBaseDataProvider)));

        public DataBaseDataProvider.DataBaseDataProvider DataProvider
        {
            get => (DataBaseDataProvider.DataBaseDataProvider)GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public DataBaseDataProviderControl()
        {
            InitializeComponent();
        }

        public DataBaseDataProviderControl(DataBaseDataProvider.DataBaseDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }

        private void SelectInstance_OnClick(object sender, RoutedEventArgs e)
        {
//            var selectInstanceWindow = new SelectInstanceWindow {Owner = Application.Current.MainWindow};
//            if (selectInstanceWindow.ShowDialog() == true)
//                DataProvider.LoadData(selectInstanceWindow.SelectedLaunch.Id);
//            else SessionTitle.Text = string.Empty;
        }
    }
}
