using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Board;
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

            response = GetResponse(board);

            return true;
        }

        public string GetResponse(Board board)
        {
            var direction = Direction.Unknown;

            //            board.Paths.Add(board.Border.GetAlongPath(board.Border.FirstOrDefault(c => c.IsBoundary)?.Position).ToArray());

            //            board.Paths.Add(board.Border.Where(c => c.IsBoundary).Select(c => c.Position).ToArray());

            var point = board.Player.Position[Direction.Up];

            board.Paths.Add(CreatePath(board, point, Direction.Up, 1, 1).ToArray());

            return $"{{\"command\": \"{direction.GetCommand()}\"}}";
        }

//        public IEnumerable<int[]> GetPathLengths(Board board, Point point, Direction direction, RoundDirection roundDirection)
//        {
//            var points = point.GetLine(direction, board.Size).While(p => board.EnemiesMap(p) > board.Player.Map[p].Weight);
//
//
//            return new[] {points};
//        }

        public IEnumerable<Point> CreatePath(Board board, Point point, Direction direction, params int[] lengths)
        {
            var corner = point;
            if (lengths.Length == 0)
            {
                foreach (var p in point.GetLine(direction.Clockwise(2), board.Size))
                {
                    yield return p;
                    if (board[p].Element == Element.ME_TERRITORY) yield break;
                }
            }

            
            foreach (var p in point.GetLine(direction, board.Size, lengths.First()))
                yield return corner = p;

            foreach (var p2 in CreatePath(board, corner, direction.Clockwise(2), lengths.Skip(1).ToArray()))
                yield return p2;
        }


        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);
    }
}