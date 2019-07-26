using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BotBase.Annotations;
using BotBase.Interfaces;

namespace BotBase.BotInstance
{
    [Serializable]
    public class BotInstance : ILogger, ISerializable, INotifyPropertyChanged
    {
        private ISolver _solver;
        private IDataLogger _dataLogger;
        private IDataProvider _dataProvider;

        private bool _isStarted;

        public bool IsStarted
        {
            get => _isStarted;
            private set
            {
                if (value == _isStarted) return;
                _isStarted = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartTime { get; private set; }
        public string Title => $"[{Solver?.GetType().Name ?? "EMPTY SOLVER"}] {DataProvider?.Title ?? "EMPTY PROVIDER"}";
        public string Name => DataProvider?.Name ?? "NOT INITIALIZED";

        public int? SettingsId { get; set; }

        public ISolver Solver
        {
            get => _solver;
            set
            {

                if (_solver != null)
                {
                    _solver.LogDataReceived -= SolverOnLogDataReceived;
                }

                _solver = value;

                if (_solver != null)
                {
                    _solver.LogDataReceived += SolverOnLogDataReceived;
                }

                OnPropertyChanged();
            }
        }

        public IDataProvider DataProvider
        {
            get => _dataProvider;
            set
            {
                if (_dataProvider != null)
                {
                    _dataProvider.DataReceived -= DataProviderOnDataReceived;
                    _dataProvider.LogDataReceived -= DataProviderOnLogDataReceived;

                    _dataProvider.Started -= DataProviderOnStarted;
                    _dataProvider.Stopped -= DataProviderOnStopped;
                }

                _dataProvider = value;

                if (_dataProvider != null)
                {
                    _dataProvider.DataReceived += DataProviderOnDataReceived;
                    _dataProvider.LogDataReceived += DataProviderOnLogDataReceived;

                    _dataProvider.Started += DataProviderOnStarted;
                    _dataProvider.Stopped += DataProviderOnStopped;
                }

                OnPropertyChanged();

            }
        }

        public IDataLogger DataLogger
        {
            get => _dataLogger;
            set
            {
                if (_dataLogger != null)
                {
                    _dataLogger.LogDataReceived -= DataLoggerOnLogDataReceived;
                }

                _dataLogger = value;

                if (_dataLogger != null)
                {
                    _dataLogger.LogDataReceived += DataLoggerOnLogDataReceived;
                }

                OnPropertyChanged();
            }
        }
        public BotInstance()
        {
            //DataLogger = new FileSystemDataLogger();
        }

        public BotInstance(IDataProvider dataProvider, ISolver solver) : this()
        {
            Solver = solver;
            DataProvider = dataProvider;
        }

        protected BotInstance(SerializationInfo info, StreamingContext context) : this()
        {
            var solverTypeName = info.GetString("SolverType");
            if (!string.IsNullOrEmpty(solverTypeName))
            {
                var solverType = PluginLoader.LoadType(solverTypeName);

                Solver = (ISolver)info.GetValue("Solver", solverType);
            }

            var dataLoggerTypeName = info.GetString("DataLoggerType");
            if (!string.IsNullOrEmpty(dataLoggerTypeName))
            {
                var dataLoggerType = PluginLoader.LoadType(dataLoggerTypeName);

                DataLogger = (IDataLogger)info.GetValue("DataLogger", dataLoggerType);

            }

            var dataProviderTypeName = info.GetString("DataProviderType");
            if (!string.IsNullOrEmpty(dataProviderTypeName))
            {
                var dataProviderType = PluginLoader.LoadType(dataProviderTypeName);

                DataProvider = (IDataProvider)info.GetValue("DataProvider", dataProviderType);

            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Solver", Solver);
            info.AddValue("SolverType", Solver?.GetType().FullName);

            info.AddValue("DataLogger", DataLogger);
            info.AddValue("DataLoggerType", DataLogger?.GetType().FullName);

            info.AddValue("DataProvider", DataProvider);
            info.AddValue("DataProviderType", DataProvider?.GetType().FullName);

            //info.AddValue("LogFilters", LogFilterEntries.ToArray());
        }

        public void Start()
        {
            StartTime = DateTime.Now;

            Solver.Initialize();

            DataProvider.Start();
        }

        public void Stop()
        {
            DataProvider?.Stop();
        }

        private void SolverOnLogDataReceived(object sender, LogRecord logRecord) => OnLogDataReceived(sender, logRecord);

        private void DataProviderOnStarted(object sender, EventArgs e)
        {
//            using (var db = new BotDbContext())
//            {
//                var hashCode = GetHashCode();
//                var settings = db.LaunchSettingsModels.Find(SettingsId);
//                if (settings == null || settings.HashCode != hashCode)
//                {
//                    if (settings != null)
//                        settings.Visibility = false;
//
//                    settings = db.LaunchSettingsModels.FirstOrDefault(t => t.HashCode == hashCode);
//                    if (settings == null)
//                    {
//                        settings = GetSettings(this);
//                        db.LaunchSettingsModels.Add(settings);
//                    }
//                }
//
//                settings.Visibility = true;
//
//                var launch = new LaunchModel()
//                {
//                    LaunchTime = StartTime,
//                    BotInstanceName = Name,
//                    BotInstanceTitle = Title,
//                    LaunchSettingsModel = settings
//                };
//
//                db.LaunchModels.Add(launch);
//
//                db.SaveChanges();
//
//                SettingsId = settings.Id;
//                LaunchId = launch.Id;
//            }

            OnStarted(DataProvider);

            IsStarted = true;
        }
        private void DataProviderOnStopped(object sender, EventArgs e)
        {
            OnStopped(DataProvider);

            IsStarted = false;
        }

        private void DataProviderOnLogDataReceived(object sender, LogRecord logRecord) => OnLogDataReceived(sender, logRecord);
        private void DataLoggerOnLogDataReceived(object sender, LogRecord logRecord) => OnLogDataReceived(sender, logRecord);
        private void DataProviderOnDataReceived(object sender, DataFrame frame)
        {
            try
            {
                DataLogger.Log(Name, StartTime, frame);

                if (Solver.Answer(Name, StartTime, frame, out var response))
                {
                    OnLogDataReceived(Solver, new LogRecord(frame, $"Response: {response}"));

                    DataProvider.SendResponse(response);

                    DataLogger.Log(Name, StartTime, frame.Time, response);
                }
                else OnLogDataReceived(Solver, new LogRecord(frame, $"Response skip"));
            }
            catch (Exception e)
            {
                DataLogger.Log(Name, StartTime, frame, e);
#if DEBUG
                //throw new Exception("Exception in DataProviderOnDataReceived", e);
                OnLogDataReceived(this, new LogRecord(frame, $"EXCEPTION: {e}"));
#endif
            }
        }

        public event EventHandler<IDataProvider> Started;
        public event EventHandler<IDataProvider> Stopped;

        public event EventHandler<LogRecord> LogDataReceived;

        protected virtual void OnLogDataReceived(object sender, LogRecord e) => LogDataReceived?.Invoke(sender, e);
        protected virtual void OnStarted(IDataProvider e) => Started?.Invoke(this, e);
        protected virtual void OnStopped(IDataProvider e) => Stopped?.Invoke(this, e);
        protected bool Equals(BotInstance other)
        {
            return Equals(_solver, other._solver) && Equals(_dataLogger, other._dataLogger) && Equals(_dataProvider, other._dataProvider);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BotInstance)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_solver != null ? _solver.GetType().Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_dataLogger != null ? _dataLogger.GetType().Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_dataProvider != null ? _dataProvider.GetHashCode() : 0);
                return hashCode;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
