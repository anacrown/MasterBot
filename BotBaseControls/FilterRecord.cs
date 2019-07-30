using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BotBase.Properties;

namespace BotBaseControls
{
    [Serializable]
    public class FilterRecord : INotifyPropertyChanged
    {
        private bool _isEnable;
        private string _header;

        public bool IsEnabled
        {
            get => _isEnable;
            set
            {
                if (value == _isEnable) return;
                _isEnable = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}