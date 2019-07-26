using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using DataBaseDataProvider.Model;
using DataBaseDataProviderView.Annotations;

namespace DataBaseDataProviderView
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

        public static readonly DependencyProperty SelectedLaunchProperty = DependencyProperty.Register(
            "SelectedLaunch", typeof(LaunchViewModel), typeof(SelectInstanceWindow), new PropertyMetadata(default(LaunchViewModel)));

        public LaunchViewModel SelectedLaunch
        {
            get => (LaunchViewModel) GetValue(SelectedLaunchProperty);
            set => SetValue(SelectedLaunchProperty, value);
        }

        public SelectInstanceWindow()
        {
            InitializeComponent();
        }

        private void SelectInstanceWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //            _db = new CodenjoyDbContext();
            //           _db.LaunchModels.Include(t => t.Frames).Load();

//            using (var db = new BotDbContext())
//            {
//                Settings = new ObservableCollection<SettingsViewModel>(db.LaunchSettingsModels.Include(t => t.Launches).Select(t => new SettingsViewModel(t)));
//            }
            
            
        }

        private void SelectInstanceWindow_OnClosing(object sender, CancelEventArgs e)
        {
//            _db.Dispose();
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedLaunch = e.NewValue as LaunchViewModel;
            OKButton.IsEnabled = SelectedLaunch != null;
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CANCELButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string _title;
        private DateTime _createTime;
        private int _launchCount;
        private ObservableCollection<LaunchViewModel> _launches;
        private int _id;

        public SettingsViewModel()
        {
            
        }

        public SettingsViewModel(LaunchSettingsModel model) : this()
        {
            Id = model.Id;
            Title = model.Title;
            CreateTime = model.CreateTime;
            LaunchCount = model.Launches.Count;

            Launches = new ObservableCollection<LaunchViewModel>(model.Launches.Select(t => new LaunchViewModel(t)));
        }

        public int Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
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

        public override string ToString() => $"{CreateTime.ToString(DataBaseDataProvider.DataBaseDataProvider.DataFormat)}: {Title} ({LaunchCount})";

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LaunchViewModel : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _launchTime;
        private int _id;
        private string _title;
        private string _header;

        public LaunchViewModel()
        {
            
        }

        public LaunchViewModel(LaunchModel model) : this()
        {
            Id = model.Id;
            Name = model.BotInstanceName;
            Title = model.BotInstanceTitle;
            LaunchTime = model.LaunchTime;
            Header = $"{Name} {LaunchTime.ToString(DataBaseDataProvider.DataBaseDataProvider.DataFormat)} {Title}";
        }

        public int Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Header
        {
            get => _header;
            set
            {
                if (value == _header) return;
                _header = value;
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
