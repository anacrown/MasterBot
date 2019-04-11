using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using CodenjoyBot.Annotations;

namespace CodenjoyBot.DataProvider.WebSocketDataProvider
{
    public partial class WebSocketDataProviderControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty DataProviderProperty = DependencyProperty.Register(
            "DataProvider", typeof(WebSocketDataProvider), typeof(WebSocketDataProviderControl),
            new PropertyMetadata(default(WebSocketDataProvider)));

        public WebSocketDataProvider DataProvider
        {
            get => (WebSocketDataProvider)GetValue(DataProviderProperty);
            set => SetValue(DataProviderProperty, value);
        }

        public WebSocketDataProviderControl()
        {
            InitializeComponent();
        }

        public WebSocketDataProviderControl(WebSocketDataProvider dataProvider) : this()
        {
            DataProvider = dataProvider;
        }

        public string ServerUri
        {
            get => DataProvider?.IdentityUser.ServerUri;
            set
            {
                if (value == DataProvider?.IdentityUser.ServerUri) return;
                if (DataProvider != null) DataProvider.IdentityUser.ServerUri = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Uri));
            }
        }

        public string UserName
        {
            get => DataProvider?.IdentityUser.UserName;
            set
            {
                if (value == DataProvider?.IdentityUser.UserName) return;
                if (DataProvider != null) DataProvider.IdentityUser.UserName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Uri));
            }
        }

        public string SecretCode
        {
            get => DataProvider?.IdentityUser.SecretCode;
            set
            {
                if (value == DataProvider?.IdentityUser.SecretCode) return;
                if (DataProvider != null) DataProvider.IdentityUser.SecretCode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Uri));
            }
        }

        public string Uri
        {
            get => DataProvider?.IdentityUser.ToUriString();
            set
            {
                DataProvider?.IdentityUser.ParseUri(value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(ServerUri));
                OnPropertyChanged(nameof(UserName));
                OnPropertyChanged(nameof(SecretCode));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GoToLinkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataProvider != null)
                Process.Start(DataProvider.IdentityUser.ToUriString());
        }
    }
}
