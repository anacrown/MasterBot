using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CodenjoyBot.DataProvider.DataBaseDataLogger;
using CodenjoyBot.DataProvider.DataBaseModel;

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
            (new SelectInstanceWindow()).ShowDialog();
        }
    }
}
