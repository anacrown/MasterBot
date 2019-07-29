using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;
using FileSystemDataProvider.Annotations;

namespace FileSystemDataProvider
{
    [Serializable]
    public class FileSystemDataProviderSettings: DataProviderSettingsBase, INotifyPropertyChanged
    {
        private string _boardFile;

        public string BoardFile
        {
            get => _boardFile;
            set
            {
                if (value == _boardFile) return;
                _boardFile = value;
                OnPropertyChanged();
            }
        }

        public string DataFormat { get; set; } = "yyyy.MM.dd hh.mm.ss";

        public FileSystemDataProviderSettings() : base()
        {
        }

        protected FileSystemDataProviderSettings(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            BoardFile = info.GetString("BoardFile");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BoardFile", BoardFile);
        }

        public override IDataProvider CreateDataProvider() => new FileSystemDataProvider(this);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}