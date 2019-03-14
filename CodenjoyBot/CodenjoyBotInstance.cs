using System;
using CodenjoyBot.DataProvider;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot
{
    public class CodenjoyBotInstance : ILogger
    {
        private ISolver _solver;
        private IDataProvider _dataProvider;

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

        public CodenjoyBotInstance(IDataProvider dataProvider = null, ISolver solver = null)
        {
            _solver = solver;
            DataProvider = dataProvider;
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
            DataProvider.Stop();

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
    }
}
