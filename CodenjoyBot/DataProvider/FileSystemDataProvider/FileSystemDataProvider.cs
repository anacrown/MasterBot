using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.DataProvider
{
    public class FileSystemDataProvider : IDataProvider
    {
        public UIElement Control { get; }
        public UIElement DebugControl { get; }

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

        private Dictionary<uint, string> _boards;
        private static readonly Regex Pattern = new Regex(@"^\[(\d*)\]:\s(.*)$");
        private readonly Timer _timer = new Timer(700);

        public FileSystemDataProvider()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += TimerOnElapsed;
        }

        public FileSystemDataProvider(string boardFile) : this()
        {
            BoardFile = boardFile;
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
            if (Time >= FrameCount)
            {
                _timer.Stop();
                return;
            }

            OnDataReceived(_boards[Time], Time);
            OnIndexChanged(Time++);
        }

        public void Start()
        {
            if (!File.Exists(BoardFile))
                throw new Exception();

            Time = 0;
            OnIndexChanged(Time);
            _boards = File.ReadAllLines(BoardFile).Select(ProcessMessage)
                .ToDictionary(frame => frame.Time, frame => frame.Board);
        }

        public void Stop()
        {
            _timer.Stop();
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
            Time = time;
            OnIndexChanged(Time);
            OnDataReceived(_boards[Time], time);

            Time++;
        }

        public void SendResponse(string response)
        {
            //throw new NotImplementedException();
        }

        public event EventHandler<uint> TimeChanged;

        public event EventHandler<DataFrame> DataReceived;

        protected virtual void OnIndexChanged(uint time) => TimeChanged?.Invoke(this, time);

        protected virtual void OnDataReceived(string board, uint time) => DataReceived?.Invoke(this, new DataFrame { Board = board, Time = time });
        public event EventHandler<LogRecord> LogDataReceived;
    }
}