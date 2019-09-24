using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Timers;
using BotBase;
using BotBase.Interfaces;
using BotBase.Properties;
using CodenjoyBot;

namespace DataBaseDataProvider
{
    [Serializable]
    public class DataBaseDataProvider : IDataProvider, INotifyPropertyChanged
    {
        private int? _launchId;
        private readonly Timer _timer = new Timer(50);

        private object _lock = new object();
        private BotDbContext _dbContext;
        private uint _frameCount;
        private string _title;

        public static string DataFormat { get; set; } = "yyyy.MM.dd hh.mm.ss";

        public DataBaseDataProvider()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += TimerOnElapsed;
        }

        protected DataBaseDataProvider(SerializationInfo info, StreamingContext context) : this()
        {
            _launchId = (int?)info.GetValue("LaunchId", typeof(int?));

            if (_launchId != null)
                LoadData(_launchId.Value);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LaunchId", _launchId);
        }

        public string Title
        {
            get => _title;
            private set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Name { get; private set; }
        public uint FrameNumber { get; private set; }

        public uint FrameCount
        {
            get => _frameCount;
            private set
            {
                if (value == _frameCount) return;
                _frameCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FrameMaximumKey));
            }
        }

        public uint FrameMaximumKey => Math.Max(FrameCount - 1, 0);

        public void LoadData(int launchId)
        {
            using (var db = new BotDbContext())
            {
                var launch = db.LaunchModels.Find(launchId);
                if (launch == null)
                    throw new Exception("Launch not found");

                _launchId = launchId;
                Name = $"{launch.BotInstanceName} {launch.LaunchTime.ToString(DataFormat)}";
                Title = launch.BotInstanceTitle;
            }
        }

        public void Start()
        {
            FrameNumber = 0;
            OnTimeChanged(FrameNumber);
            _dbContext = new BotDbContext();
            FrameCount = (uint)_dbContext.DataFrameModels.LongCount(t => t.LaunchModelId == _launchId);

            OnStarted();
        }

        public void Stop()
        {
            _dbContext?.Dispose();
            OnStopped();
        }

        public void RecordPlay()
        {
            _timer?.Start();
        }
        public void RecordStop()
        {
            _timer?.Stop();
        }
        public void MoveToFrame(uint frameNumber)
        {
            lock (_lock)
            {
                var frame = _dbContext.DataFrameModels.FirstOrDefault(t => t.LaunchModelId == _launchId && t.FrameNumber == frameNumber);

                if (frame == null)
                    throw new Exception("Frame not found");

                FrameNumber = frameNumber;
                OnTimeChanged(FrameNumber);
                OnDataReceived(new DataFrame(frame.Time, frame.Board, frame.FrameNumber));
            }
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (FrameNumber >= FrameCount)
            {
                _timer.Stop();
                return;
            }

            MoveToFrame(++FrameNumber);
            OnTimeChanged(FrameNumber);
        }

        public void SendResponse(string response)
        {

        }

        public event EventHandler Started;
        public event EventHandler Stopped;

        public event EventHandler<uint> TimeChanged;

        public event EventHandler<DataFrame> DataReceived;

        public event EventHandler<LogRecord> LogDataReceived;

        public event PropertyChangedEventHandler PropertyChanged;

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
            return Equals((DataBaseDataProvider)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        protected virtual void OnStarted() => Started?.Invoke(this, EventArgs.Empty);

        protected virtual void OnStopped() => Stopped?.Invoke(this, EventArgs.Empty);

        protected virtual void OnDataReceived(DataFrame frame) => DataReceived?.Invoke(this, frame);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
}
