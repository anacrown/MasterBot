using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;
using VisioDataProvider.Annotations;

namespace VisioDataProvider
{
    [Serializable]
    public class VisioDataProviderSettings : DataProviderSettingsBase, INotifyPropertyChanged
    {
        private string _visioFile;

        public string VisioFile
        {
            get => _visioFile;
            set
            {
                if (value == _visioFile) return;
                _visioFile = value;
                OnPropertyChanged();
            }
        }
        
        public override IDataProvider CreateDataProvider() => new VisioDataProvider(this);

        public VisioDataProviderSettings() : base() { }

        public VisioDataProviderSettings(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            VisioFile = info.GetString("VisioFile");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("VisioFile", VisioFile);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}