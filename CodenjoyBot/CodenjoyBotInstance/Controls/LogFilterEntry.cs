using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using CodenjoyBot.Annotations;

namespace CodenjoyBot.CodenjoyBotInstance.Controls
{
    [Serializable]
    public class LogFilterEntry : INotifyPropertyChanged, ISerializable
    {
        private bool? _isEnable;
        private string _header;

        public bool? IsEnabled
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

        public LogFilterEntry() { }

        protected LogFilterEntry(SerializationInfo info, StreamingContext context) : this()
        {
            Header = info.GetString("Header");
            IsEnabled = (bool?)info.GetValue("IsEnabled", typeof(bool?));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Header", Header);
            info.AddValue("IsEnabled", IsEnabled);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
