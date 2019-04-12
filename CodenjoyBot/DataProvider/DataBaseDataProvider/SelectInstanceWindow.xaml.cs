using System.ComponentModel;
using System.Windows;
using CodenjoyBot.DataProvider.DataBaseDataLogger;
using Microsoft.EntityFrameworkCore;

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
