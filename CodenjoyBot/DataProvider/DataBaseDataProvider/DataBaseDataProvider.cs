using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Timers;
using System.Windows;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.DataProvider.DataBaseDataProvider
{
    [Serializable]
    public class DataBaseDataProvider : IDataProvider
    {
        private int? _launchId;
        private UIElement _control;
        private UIElement _debugControl;
        private readonly Timer _timer = new Timer(50);

        private CodenjoyDbContext _dbContext;

        public UIElement Control => _control ?? (_control = new DataBaseDataProviderControl(this));

        public UIElement DebugControl => _debugControl ?? (_debugControl = new DataBaseDataProviderDebugControl(this));

        public DataBaseDataProvider()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += TimerOnElapsed;
        }

        protected DataBaseDataProvider(SerializationInfo info, StreamingContext context) : this() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

        public string Title { get; private set; }
        public string Name { get; private set; }
        public uint Time { get; private set; }

        public long FrameCount { get; private set; }

        public void LoadData(int launchId)
        {
            using (var db = new CodenjoyDbContext())
            {
                var launch = db.LaunchModels.Find(launchId);
                if (launch == null)
                    throw new Exception("Launch not found");

                _launchId = launchId;
                Name = $"{launch.BotInstanceName} {launch.LaunchTime.ToString(Properties.Resources.DateFormat)}";
                Title = launch.BotInstanceTitle;
            }
        }

        public void Start()
        {
            Time = 0;
            OnTimeChanged(Time);
            _dbContext = new CodenjoyDbContext();
            FrameCount = _dbContext.DataFrameModels.LongCount(t => t.LaunchModelId == _launchId);
        }

        public void Stop()
        {
            _dbContext.Dispose();
        }

        public void RecordPlay()
        {
            _timer?.Start();
        }
        public void RecordStop()
        {
            _timer?.Stop();
        }
        public void MoveToFrame(uint time)
        {
            var frame = _dbContext.DataFrameModels.FirstOrDefault(t => t.LaunchModelId == _launchId && t.Time == time);
            if (frame == null)
                throw new Exception("Frame not found");

            Time = time;
            OnTimeChanged(Time);
            OnDataReceived(frame.Board, time);

            Time++;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (Time >= FrameCount)
            {
                _timer.Stop();
                return;
            }

            MoveToFrame(++Time);
            OnTimeChanged(Time);
        }

        public void SendResponse(string response)
        {
            
        }

        public event EventHandler Started;
        public event EventHandler Stopped;

        public event EventHandler<uint> TimeChanged;

        public event EventHandler<DataFrame> DataReceived;

        public event EventHandler<LogRecord> LogDataReceived;

        protected virtual void OnTimeChanged(uint e) => TimeChanged?.Invoke(this, e);

        protected bool Equals(DataBaseDataProvider other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataBaseDataProvider) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        protected virtual void OnDataReceived(string board, uint time) => DataReceived?.Invoke(this, new DataFrame(){Board = board, Time = time});
    }
}
