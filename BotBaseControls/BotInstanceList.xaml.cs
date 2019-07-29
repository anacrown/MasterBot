using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BotBase;

namespace BotBaseControls
{
    public partial class BotInstanceList : UserControl
    {
        public static readonly DependencyProperty InstanceModelsProperty = DependencyProperty.Register(
            "InstanceModels", typeof(ObservableCollection<BotInstance>), typeof(BotInstanceList), new PropertyMetadata(default(ObservableCollection<BotInstance>)));

        public ObservableCollection<BotInstance> InstanceModels
        {
            get => (ObservableCollection<BotInstance>)GetValue(InstanceModelsProperty);
            set => SetValue(InstanceModelsProperty, value);
        }

        public event EventHandler<BotInstance> RemoveInstance;

        public BotInstanceList()
        {
            InitializeComponent();
        }

        private void AddBotInstance_OnClick(object sender, RoutedEventArgs e)
        {
            if (InstanceModels == null)
                InstanceModels = new ObservableCollection<BotInstance>();

            InstanceModels.Add(new BotInstance(new BotInstanceSettings()));
        }

        private void RemoveBotInstance_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItem is BotInstance selectedItem)
            {
                RemoveInstance?.Invoke(this, selectedItem);
                InstanceModels.Remove(selectedItem);
            }
        }
    }
}
