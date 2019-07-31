using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;
using Newtonsoft.Json.Linq;
using VisioDataProvider.Annotations;
using System.IO.Compression;

namespace VisioDataProvider
{
    [Serializable]
    public class VisioDataProvider : IDataProvider, ISerializable, INotifyPropertyChanged
    {
        private Dictionary<uint, JToken> _boards;
        private uint _frameNumber;
        private string _name;
        private int _playersCount;
        private int _currentPlayer;

        public VisioDataProviderSettings Settings { get; }

        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<uint> TimeChanged;
        public event EventHandler<DataFrame> DataReceived;
        public event EventHandler<LogRecord> LogDataReceived;

        public VisioDataProvider(VisioDataProviderSettings settings)
        {
            Settings = settings;

            Name = $"VisioDataProvider";
        }

        public VisioDataProvider(SerializationInfo info, StreamingContext context) : this((VisioDataProviderSettings)info.GetValue("Settings", typeof(VisioDataProviderSettings))) { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue("Settings", Settings);

        public string Title => Settings.VisioFile;

        public string Name
        {
            get => _name;
            private set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public uint FrameNumber
        {
            get => _frameNumber;
            private set
            {
                if (value == _frameNumber) return;
                _frameNumber = value;
                OnPropertyChanged();
            }
        }

        public int FrameCount => _boards?.Count ?? 0;
        public int FrameMaximumKey => _boards?.Count - 1 ?? 0;

        public int PlayersCount
        {
            get => _playersCount;
            set
            {
                if (value == _playersCount) return;
                _playersCount = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (value == _currentPlayer) return;
                _currentPlayer = value;
                OnPropertyChanged();
            }
        }

        public void Start()
        {
            if (!File.Exists(Settings.VisioFile))
                return; //throw new Exception();

            var startTime = DateTime.Now;

            var visioFile = Settings.VisioFile;
            if (Path.GetExtension(Settings.VisioFile) == ".gz")
            {
                visioFile = Path.Combine(FileSystemConfigurator.AppDataDir, Path.GetFileNameWithoutExtension(Settings.VisioFile));
                using (FileStream sourceStream = new FileStream(Settings.VisioFile, FileMode.OpenOrCreate))
                using (FileStream targetStream = File.Create(visioFile))
                using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    decompressionStream.CopyTo(targetStream);
            }

            JObject jObject = JObject.Parse(File.ReadAllText(visioFile));

            _boards = new Dictionary<uint, JToken>();

            uint tick = 0;
            foreach (var token in jObject["visio_info"])
            {
                var t = new JObject
                {
                    ["type"] = token["type"]
                };

                if (t["type"].Value<string>() != "end_game")
                {
                    if (tick == 0)
                    {
                        t["params"] = new JObject
                        {
                            ["x_cells_count"] = token["x_cells_count"],
                            ["y_cells_count"] = token["y_cells_count"],
                            ["speed"] = token["speed"],
                            ["width"] = token["width"],
                            ["x_cells_count"] = token["x_cells_count"],
                            ["y_cells_count"] = token["y_cells_count"],
                            ["speed"] = token["speed"],
                            ["width"] = token["width"]
                        };
                    }
                    else
                    {
                        t["params"] = new JObject
                        {
                            ["players"] = token["players"],
                            ["bonuses"] = token["bonuses"],
                            ["tick_num"] = token["tick_num"]
                        };
                    }
                }

                _boards.Add(tick, t);

                tick++;
            }

            PlayersCount = _boards[1]["params"]["players"].Count();

            //            var responseFilePath = Path.Combine(Path.GetDirectoryName(Settings.BoardFile), "Response.txt");
            //            if (File.Exists(responseFilePath))
            //                _responses = File.ReadAllLines(responseFilePath).Select(ProcessMessage).ToDictionary(frame => frame.FrameNumber, frame => frame.Board);

            MoveToFrame(0);

            OnPropertyChanged(nameof(FrameCount));
            OnPropertyChanged(nameof(FrameMaximumKey));

            OnStarted();
        }

        public void Stop()
        {
            //            throw new NotImplementedException();
        }

        public void SendResponse(string response)
        {
            //throw new NotImplementedException();
        }

        public void MoveToFrame(uint frameNumber)
        {
            if (frameNumber > FrameMaximumKey)
                return;

            FrameNumber = frameNumber;
            OnTimeChanged(FrameNumber);

            if (CurrentPlayer != 0 && 
                _boards[FrameNumber]["type"].Value<string>() != "start_game" &&
                _boards[FrameNumber]["type"].Value<string>() != "end_game" )
            {
                var players = new JObject();
                foreach (JProperty token in _boards[FrameNumber]["params"]["players"])
                {
                    if (token.Name == CurrentPlayer.ToString())
                    {
                        var player = new JProperty("i", token.Value);
                        players.Add(player);
                    }
                    else
                    {
                        players.Add(token);
                    }
                }

                _boards[FrameNumber]["params"]["players"] = players;
            }

            OnDataReceived(new DataFrame(DateTime.Now, _boards[FrameNumber].ToString(), FrameNumber));

            //            if (_responses != null)
            //            {
            //                var response = _responses.ContainsKey(FrameNumber) ? _responses[FrameNumber] : "NOT RESPONSE";
            //                OnLogDataReceived(new LogRecord(_boards[FrameNumber], $"Response {response}"));
            //            }
        }

        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);

        protected virtual void OnStarted() => Started?.Invoke(this, EventArgs.Empty);

        protected virtual void OnStopped() => Stopped?.Invoke(this, EventArgs.Empty);

        protected virtual void OnTimeChanged(uint e) => TimeChanged?.Invoke(this, e);

        protected virtual void OnDataReceived(DataFrame e) => DataReceived?.Invoke(this, e);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
