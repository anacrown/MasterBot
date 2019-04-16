using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using CodenjoyBot.Annotations;
using CodenjoyBot.Entity;
using Microsoft.EntityFrameworkCore;

namespace CodenjoyBot.DataProvider.DataBaseDataProvider
{
    /// <summary>
    /// Interaction logic for SelectInstanceWindow.xaml
    /// </summary>
    public partial class SelectInstanceWindow : Window
    {
//        private CodenjoyDbContext _db;

        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            "Settings", typeof(ObservableCollection<SettingsViewModel>), typeof(SelectInstanceWindow), new PropertyMetadata(default(ObservableCollection<SettingsViewModel>)));

        public ObservableCollection<SettingsViewModel> Settings
        {
            get => (ObservableCollection<SettingsViewModel>) GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public SelectInstanceWindow()
        {
            InitializeComponent();
        }

        private void SelectInstanceWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //            _db = new CodenjoyDbContext();
            //           _db.LaunchModels.Include(t => t.Frames).Load();

            using (var db = new CodenjoyDbContext())
            {
                Settings = new ObservableCollection<SettingsViewModel>(db.LaunchSettingsModels.Include(t => t.Launches).Select(t => new SettingsViewModel(t)));
            }
            
            
        }

        private void SelectInstanceWindow_OnClosing(object sender, CancelEventArgs e)
        {
//            _db.Dispose();
        }
    }

    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string _title;
        private DateTime _createTime;
        private int _launchCount;
        private ObservableCollection<LaunchViewModel> _launches;

        public SettingsViewModel()
        {
            
        }

        public SettingsViewModel(LaunchSettingsModel model) : this()
        {
            Title = model.Title;
            CreateTime = model.CreateTime;
            LaunchCount = model.Launches.Count;

            Launches = new ObservableCollection<LaunchViewModel>(model.Launches.Select(t => new LaunchViewModel(t)));
        }

        public ObservableCollection<LaunchViewModel> Launches
        {
            get => _launches;
            set
            {
                if (Equals(value, _launches)) return;
                _launches = value;
                OnPropertyChanged();
            }
        }
        
        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public DateTime CreateTime
        {
            get => _createTime;
            set
            {
                if (value.Equals(_createTime)) return;
                _createTime = value;
                OnPropertyChanged();
            }
        }

        public int LaunchCount
        {
            get => _launchCount;
            set
            {
                if (value == _launchCount) return;
                _launchCount = value;
                OnPropertyChanged();
            }
        }

        public override string ToString() => $"{CreateTime.ToString(Properties.Resources.DateFormat)}: {Title} ({LaunchCount})";

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class LaunchViewModel : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _launchTime;

        public LaunchViewModel()
        {
            
        }

        public LaunchViewModel(LaunchModel model) : this()
        {
            Name = model.BotInstanceName;
            LaunchTime = model.LaunchTime;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public DateTime LaunchTime
        {
            get => _launchTime;
            set
            {
                if (value.Equals(_launchTime)) return;
                _launchTime = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
