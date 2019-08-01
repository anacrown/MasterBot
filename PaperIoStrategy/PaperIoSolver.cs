using System;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;
using Newtonsoft.Json;
using PaperIoStrategy.AISolver;
using PaperIoStrategy.DataContract;

namespace PaperIoStrategy
{
    public class PaperIoSolver : ISolver
    {
        public PaperIoSolverSettings Settings { get; set; }

        private JPacket _startInfo;

        public event EventHandler<Board> BoardChanged;
        public event EventHandler<LogRecord> LogDataReceived;

        public PaperIoSolver(PaperIoSolverSettings settings)
        {
            Settings = settings;
        }

        public PaperIoSolver(SerializationInfo info, StreamingContext context) : this(info.GetValue("Settings", typeof(PaperIoSolverSettings)) as PaperIoSolverSettings)
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Settings", Settings);
        }

        public void Initialize()
        {

        }

        protected virtual void OnBoardChanged(Board e) => BoardChanged?.Invoke(this, e);

        public bool Answer(string instanceName, DateTime startTime, DataFrame frame, out string response)
        {
            response = string.Empty;
            var jPacket = JsonConvert.DeserializeObject<JPacket>(frame.Board);

            if (jPacket.PacketType == JPacketType.EndGame)
            {
                return false;
            }

            var board = new Board(instanceName, startTime, frame, jPacket.PacketType == JPacketType.StartGame ? _startInfo = jPacket : jPacket.Merge(_startInfo));
            OnBoardChanged(board);

            if (board.Players != null)
            {
                foreach (var player in board.Players)
                {
                    OnLogDataReceived(new LogRecord(frame, $"Player {player.Key}: {player.Value.Position} V({player.Value.Speed}) {player.Value.JPlayer.Position} {player.Value.Direction}"));
                    foreach (var bonus in player.Value.Bonuses)
                    {
                        OnLogDataReceived(new LogRecord(frame, $"({board.JPacket.Params.Tick}) Bonus {bonus.BonusType}; Moves: {bonus.Moves}; Ticks: {bonus.Pixels}"));
                    }
                }
            }

            if (board.JPacket.PacketType != JPacketType.Tick) return false;

            response = board.GetResponse();

            return true;
        }

        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);
    }
}