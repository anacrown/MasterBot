using CodenjoyBot.CodenjoyBotInstance.Controls;
using CodenjoyBot.DataProvider;
using CodenjoyBot.DataProvider.FileSystemDataLogger;
using CodenjoyBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Windows;

namespace CodenjoyBot.CodenjoyBotInstance
{
    [Serializable]
    public class CodenjoyBotInstance : ILogger, ISupportControls, ISerializable
    {
        private ISolver _solver;
        private IDataLogger _dataLogger;
        private IDataProvider _dataProvider;
        private readonly LogFilterEntry[] _logFilterEntries;

        private UIElement _control;
        private UIElement _debugControl;

        public UIElement Control => _control ?? (_control = new CodenjoyBotInstanceControl(this));
        public UIElement DebugControl => _debugControl ?? (_debugControl = new CodenjoyBotInstanceDebugControl(this, _logFilterEntries));

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
            }
        }

        private void SolverOnLogDataReceived(object sender, LogRecord logRecord)
        {
            OnLogDataReceived(sender, logRecord);
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

            }
        }

        private void DataProviderOnStarted(object sender, EventArgs e) => OnStarted(DataProvider);

        private void DataProviderOnStopped(object sender, EventArgs e) => OnStopped(DataProvider);

        private void DataProviderOnLogDataReceived(object sender, LogRecord logRecord)
        {
            OnLogDataReceived(sender, logRecord);
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
            }
        }

        private void DataLoggerOnLogDataReceived(object sender, LogRecord logRecord)
        {
            OnLogDataReceived(sender, logRecord);
        }

        public bool IsStarted { get; private set; }
        public DateTime StartTime { get; private set; }

        public string Title => $"[{Solver?.GetType().Name ?? "EMPTY SOLVER"}] {DataProvider?.Title ?? "EMPTY PROVIDER"}";

        public string Name => DataProvider?.Name ?? "NOT INITIALIZED";

        public CodenjoyBotInstance()
        {
            //WebSocketDataLogger.Instance.LogDataReceived += InstanceOnLogDataReceived;

            DataLogger = new FileSystemDataLogger();
        }

        public CodenjoyBotInstance(IDataProvider dataProvider, ISolver solver) : this()
        {
            Solver = solver;
            DataProvider = dataProvider;
        }

        protected CodenjoyBotInstance(SerializationInfo info, StreamingContext context) : this()
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

            var filterEntries = new List<LogFilterEntry>();
            foreach (var filterEntry in (LogFilterEntry[])info.GetValue("LogFilters", typeof(LogFilterEntry[])))
            {
                if (filterEntries.Any(t => t.Header == filterEntry.Header))
                {
                    continue;
                }

                filterEntries.Add(filterEntry);
            }

            _logFilterEntries = filterEntries.ToArray();
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Solver", Solver);
            info.AddValue("SolverType", Solver?.GetType().FullName);

            info.AddValue("DataLogger", DataLogger);
            info.AddValue("DataLoggerType", DataLogger?.GetType().FullName);

            info.AddValue("DataProvider", DataProvider);
            info.AddValue("DataProviderType", DataProvider?.GetType().FullName);

            info.AddValue("LogFilters", (DebugControl as CodenjoyBotInstanceDebugControl)?.LogFilters?.ToArray());
        }

        public void Start()
        {
            Solver.Initialize();
            StartTime = DateTime.Now;

            DataProvider.Start();

            IsStarted = true;
        }

        public void Stop()
        {
            DataProvider?.Stop();

            IsStarted = false;
        }

        private void DataProviderOnDataReceived(object sender, DataFrame frame)
        {
            var response = Solver.Answer(new Board.Board(Name, StartTime, frame));

            DataProvider.SendResponse(response);

            DataLogger.Log(this, frame.Time, frame.Board, response);
        }

        public event EventHandler<IDataProvider> Started;
        public event EventHandler<IDataProvider> Stopped;

        public event EventHandler<LogRecord> LogDataReceived;

        protected virtual void OnLogDataReceived(object sender, LogRecord e)
        {
            LogDataReceived?.Invoke(sender, e);
        }

        protected virtual void OnStarted(IDataProvider e) => Started?.Invoke(this, e);

        protected virtual void OnStopped(IDataProvider e) => Stopped?.Invoke(this, e);
    }
}
