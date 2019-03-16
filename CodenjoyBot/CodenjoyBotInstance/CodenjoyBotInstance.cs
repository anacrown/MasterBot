using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Windows;
using CodenjoyBot.CodenjoyBotInstance.Controls;
using CodenjoyBot.DataProvider;
using CodenjoyBot.DataProvider.WebSocketDataProvider;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.CodenjoyBotInstance
{
    [Serializable]
    public class CodenjoyBotInstance : ILogger, ISupportControls, ISerializable
    {
        private ISolver _solver;
        private IDataProvider _dataProvider;
        private LogFilterEntry[] _logFilterEntries;

        private UIElement _control;
        private UIElement _debugControl;

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

        private void SolverOnLogDataReceived(object sender, LogRecord logRecord) =>
            OnLogDataReceived(sender, logRecord);

        public IDataProvider DataProvider
        {
            get => _dataProvider;
            set
            {
                if (_dataProvider != null)
                {
                    _dataProvider.DataReceived -= DataProviderOnDataReceived;
                    _dataProvider.LogDataReceived -= DataProviderOnLogDataReceived;
                }

                _dataProvider = value;

                if (_dataProvider != null)
                {
                    _dataProvider.DataReceived += DataProviderOnDataReceived;
                    _dataProvider.LogDataReceived += DataProviderOnLogDataReceived;
                }

            }
        }

        private void DataProviderOnLogDataReceived(object sender, LogRecord logRecord) =>
            OnLogDataReceived(sender, logRecord);

        public bool IsStarted { get; private set; }
        public DateTime StartTime { get; private set; }

        public string Name => DataProvider?.Name ?? "NOT INITIALIZED";

        public CodenjoyBotInstance()
        {
            WebSocketDataLogger.Instance.LogDataReceived += InstanceOnLogDataReceived;
        }

        public CodenjoyBotInstance(IDataProvider dataProvider, ISolver solver) : this()
        {
            Solver = solver;
            DataProvider = dataProvider;
        }

        protected CodenjoyBotInstance(SerializationInfo info, StreamingContext context): this()
        {
            var solverTypeName = info.GetString("SolverType");
            var solverType = PluginLoader.LoadType(solverTypeName);

            Solver = (ISolver) info.GetValue("Solver", solverType);

            var dataProviderTypeName = info.GetString("DataProviderType");
            var dataProviderType = PluginLoader.LoadType(dataProviderTypeName);

            DataProvider = (IDataProvider) info.GetValue("DataProvider", dataProviderType);

            _logFilterEntries = (LogFilterEntry[])info.GetValue("LogFilters", typeof(LogFilterEntry[]));
        }

        private void InstanceOnLogDataReceived(object sender, LogRecord logRecord)
        {
            if (logRecord.Message.StartsWith($"{Name}: "))
                OnLogDataReceived(sender, logRecord);
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
            //            try
            //            {
            var response = Solver.Answer(new Board.Board(Name, StartTime, frame));
            DataProvider.SendResponse(response);

            //MainWindow.Log(frame.Time, $"{Name}: {response}");
            WebSocketDataLogger.Instance.Log(Name, StartTime, frame.Time, frame.Board, response);
            //            }
            //            catch (Exception e)
            //            {
            //                WebSocketDataLogger.Instance.Log(Name, StartTime, frame.Time, e);
            //
            //                Stop();
            //
            //                Start();
            //            }
        }

        public event EventHandler<LogRecord> LogDataReceived;
        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);
        protected virtual void OnLogDataReceived(object sender, LogRecord e) => LogDataReceived?.Invoke(sender, e);

        public UIElement Control => _control ?? (_control = new CodenjoyBotInstanceControl(this));
        public UIElement DebugControl => _debugControl ?? (_debugControl = new CodenjoyBotInstanceDebugControl(this, _logFilterEntries));

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Solver", Solver);
            info.AddValue("SolverType", Solver.GetType().FullName);

            info.AddValue("DataProvider", DataProvider);
            info.AddValue("DataProviderType", DataProvider?.GetType().FullName);

            info.AddValue("LogFilters", (DebugControl as CodenjoyBotInstanceDebugControl)?.LogFilters?.ToArray());
        }
    }
}
