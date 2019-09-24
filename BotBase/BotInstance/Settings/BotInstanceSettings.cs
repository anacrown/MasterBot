using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BotBase.Properties;

namespace BotBase
{
    [Serializable]
    public class BotInstanceSettings : SettingsBase, INotifyPropertyChanged
    {
        private SolverSettingsBase _solverSettings;
        private DataLoggerSettingsBase _dataLoggerSettings;
        private DataProviderSettingsBase _dataProviderSettings;

        public SolverSettingsBase SolverSettings
        {
            get => _solverSettings;
            set
            {
                if (Equals(value, _solverSettings)) return;
                _solverSettings = value;
                OnPropertyChanged();
            }
        }

        public DataLoggerSettingsBase DataLoggerSettings
        {
            get => _dataLoggerSettings;
            set
            {
                if (Equals(value, _dataLoggerSettings)) return;
                _dataLoggerSettings = value;
                OnPropertyChanged();
            }
        }

        public DataProviderSettingsBase DataProviderSettings
        {
            get => _dataProviderSettings;
            set
            {
                if (Equals(value, _dataProviderSettings)) return;
                _dataProviderSettings = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartTime { get; set; }
        public bool Visibility { get; set; }
        public string Title { get; set; }

        public BotInstanceSettings() : base()
        {

        }

        public BotInstanceSettings(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var solverTypeName = info.GetString("SolverSettingsType");
            if (!string.IsNullOrEmpty(solverTypeName))
            {
                var solverType = PluginLoader.LoadType(solverTypeName);
                if (solverType != null)
                    SolverSettings = (SolverSettingsBase)info.GetValue("SolverSettings", solverType);
            }

            var dataLoggerTypeName = info.GetString("DataLoggerSettingsType");
            if (!string.IsNullOrEmpty(dataLoggerTypeName))
            {
                var dataLoggerType = PluginLoader.LoadType(dataLoggerTypeName);
                if (dataLoggerType != null)
                    DataLoggerSettings = (DataLoggerSettingsBase)info.GetValue("DataLoggerSettings", dataLoggerType);
            }

            var dataProviderTypeName = info.GetString("DataProviderSettingsType");
            if (!string.IsNullOrEmpty(dataProviderTypeName))
            {
                var dataProviderType = PluginLoader.LoadType(dataProviderTypeName);
                if (dataProviderType != null)
                    DataProviderSettings = (DataProviderSettingsBase)info.GetValue("DataProviderSettings", dataProviderType);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("SolverSettings", SolverSettings);
            info.AddValue("SolverSettingsType", SolverSettings?.GetType().FullName);

            info.AddValue("DataLoggerSettings", DataLoggerSettings);
            info.AddValue("DataLoggerSettingsType", DataLoggerSettings?.GetType().FullName);

            info.AddValue("DataProviderSettings", DataProviderSettings);
            info.AddValue("DataProviderSettingsType", DataProviderSettings?.GetType().FullName);

            info.AddValue("StartTime", StartTime);
            info.AddValue("Visibility", Visibility);
            info.AddValue("Title", Title);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}