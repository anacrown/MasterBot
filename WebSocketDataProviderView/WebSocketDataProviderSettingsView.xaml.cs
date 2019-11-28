using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WebSocketDataProvider;

namespace WebSocketDataProviderView
{
    /// <summary>
    /// Логика взаимодействия для WebSocketDataProviderSettingsView.xaml
    /// </summary>
    public partial class WebSocketDataProviderSettingsView : UserControl
    {
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            "Settings", typeof(WebSocketDataProviderSettings), typeof(WebSocketDataProviderSettingsView), new PropertyMetadata(default(WebSocketDataProviderSettings)));

        public WebSocketDataProviderSettings Settings
        {
            get => (WebSocketDataProviderSettings) GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public WebSocketDataProviderSettingsView()
        {
            InitializeComponent();
        }

        private void GoToLinkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings?.IdentityUser != null)
                Process.Start(Settings.IdentityUser.ToUriString());
        }
    }
}
