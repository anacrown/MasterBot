using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
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
using System.Windows.Shapes;
using CodenjoyBot.DataProvider.DataBaseDataLogger;

namespace CodenjoyBot.DataProvider.DataBaseDataProvider
{
    /// <summary>
    /// Interaction logic for SelectInstanceWindow.xaml
    /// </summary>
    public partial class SelectInstanceWindow : Window
    {
        private CodenjoyDbContext _db;

        public SelectInstanceWindow()
        {
            InitializeComponent();
        }

        private void SelectInstanceWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _db = new CodenjoyDbContext();
            _db.LaunchModels.Load();

            TreeView.ItemsSource = _db.LaunchModels.Local.ToBindingList();
        }

        private void SelectInstanceWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _db.Dispose();
        }
    }
}
