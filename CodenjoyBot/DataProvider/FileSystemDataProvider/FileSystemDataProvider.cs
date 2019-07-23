using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using CodenjoyBot.Annotations;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.DataProvider.FileSystemDataProvider
{
    [Serializable]
    public class FileSystemDataProvider : IDataProvider, INotifyPropertyChanged
    {
        public string Title => BoardFile;
        public string Name { get; private set; }
        
        private string _boardFile;
        public string BoardFile
        {
            get => _boardFile;
            set
            {
                _boardFile = value;
                Name = GetNameFromDir(_boardFile);
            }
        }

        public uint Time { get; private set; }
        public int FrameCount => _boards?.Count ?? 0;
        public int FrameMaximumKey => _boards?.Count - 1 ?? 0;

        private Dictionary<uint, string> _boards;
        private Dictionary<uint, string> _responses;
        private static readonly Regex Pattern = new Regex(@"^\[(\d*)\]:\s(.*)$");
        private readonly Timer _timer = new Timer(50);

        private UIElement _control;
        private UIElement _debugControl;
        public UIElement Control => _control ?? (_control = new FileSystemDataProviderControl(this));
        public UIElement DebugControl => _debugControl ?? (_debugControl = new FileSystemDataProviderDebugControl(this));

        public FileSystemDataProvider()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += TimerOnElapsed;
        }

        public FileSystemDataProvider(string boardFile) : this()
        {
            BoardFile = boardFile;
        }

        protected FileSystemDataProvider(SerializationInfo info, StreamingContext context) : this()
        {
            BoardFile = info.GetString("BoardFile");
        }

        private string GetNameFromDir(string file)
        {
            var dir = Path.GetDirectoryName(file);
            var startDir = Path.GetFileName(dir);

            var subDir = Path.GetDirectoryName(dir);
            var instanceDir = Path.GetFileName(subDir);

            return $"{instanceDir} {startDir}";
        }

        private static DataFrame ProcessMessage(string message)
        {
            var match = Pattern.Match(message);
            if (!match.Success)
            {
                throw new ApplicationException($"Cannot match message: '{message}'");
            }

            return new DataFrame { Board = match.Groups[2].Value, Time = uint.TryParse(match.Groups[1].Value, out uint time) ? time : 0 };
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (Time < FrameCount)
            {
                OnDataReceived(_boards[Time], Time);
                OnTimeChanged(Time++);
            }
            else _timer.Stop();
        }

        public void Start()
        {
            if (!File.Exists(BoardFile))
                throw new Exception();

            _boards = File.ReadAllLines(BoardFile).Select(ProcessMessage).ToDictionary(frame => frame.Time, frame => frame.Board);

            var responseFilePath = Path.Combine(Path.GetDirectoryName(BoardFile), "Response.txt");
            if (File.Exists(responseFilePath))
                _responses = File.ReadAllLines(responseFilePath).Select(ProcessMessage).ToDictionary(frame => frame.Time, frame => frame.Board);

            MoveToFrame(0);

            OnPropertyChanged(nameof(FrameCount));
            OnPropertyChanged(nameof(FrameMaximumKey));

            OnStarted();
        }

        public void Stop()
        {
            _timer.Stop();

            OnStopped();
        }

        public void RecordPlay() => _timer?.Start();

        public void RecordStop() => _timer?.Stop();

        public void MoveToFrame(uint time)
        {
            Time = time;
            OnTimeChanged(Time);
            OnDataReceived(_boards[Time], time);

            if (_responses != null)
            {
                var response = _responses.ContainsKey(Time) ? _responses[Time] : "NOT RESPONSE";
                OnLogDataReceived(new LogRecord(new DataFrame() {Time = Time, Board = _boards[Time]}, $"Response {response}"));
            }
        }

        public void SendResponse(string response)
        {
            //throw new NotImplementedException();
        }

        public event EventHandler<uint> TimeChanged;

        public event EventHandler<DataFrame> DataReceived;
        protected virtual void OnDataReceived(string board, uint time) => DataReceived?.Invoke(this, new DataFrame { Board = board, Time = time });

        public event EventHandler Started;
        public event EventHandler Stopped;

        public event EventHandler<LogRecord> LogDataReceived;
        public virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BoardFile", BoardFile);
        }

        protected virtual void OnStarted() => Started?.Invoke(this, EventArgs.Empty);


        //Никогда не останавливается? ...
        protected virtual void OnStopped() => Stopped?.Invoke(this, EventArgs.Empty);

        protected bool Equals(FileSystemDataProvider other)
        {
            return string.Equals(_boardFile, other._boardFile) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileSystemDataProvider) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_boardFile != null ? _boardFile.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        protected virtual void OnTimeChanged(uint e) => TimeChanged?.Invoke(this, e);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}