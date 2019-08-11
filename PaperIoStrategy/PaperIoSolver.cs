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

            if (board.Player != null)
            {
                if (board.Player.Border[board.Player.Position].IsBoundary)
                {
                    direction = board.Player.Border[board.Player.Position].OutDirections.First();
                } else if (board.Player.Border[board.Player.Position].IsTerritory)
                {
                    var backPoint = board.Player.Direction != Direction.Unknown
                        ? board.Player.Position[board.Player.Direction.Invert()]
                        : null;

                    var targetPoint = board.Player.Border
                        .Where(e => e.IsBoundary && e.Position != backPoint)
                        .MinSingle(e => board.Player.Map[e.Position].Weight).Position;

                    board.Paths.Add(new[] { targetPoint });

                    var path = board.Player.Map.Tracert(targetPoint);
                    direction = board.Player.Position.GetDirectionTo(path.First());
                }
                else
                {
                    var backPoint = board.Player.Direction != Direction.Unknown
                        ? board.Player.Position[board.Player.Direction.Invert()]
                        : null;

                    var targetPoint = board.Player.Border
                        .Where(e => e.IsBoundary && e.Position != backPoint)
                        .MinSingle(e => board.Player.Map[e.Position].Weight).Position;

                    board.Paths.Add(new[]{targetPoint});

                    var entries = board.Player.PossibleMaps
                        .Select(pair => (direction: pair.Key, score: GetScoreForMove(board, board.Player, targetPoint, pair.Key)));

                    var entry = entries.MaxSingle(e => e.score);

                    if (entry.score > 0)
                        direction = entry.direction;
                    else
                    {
                        var path = board.Player.Map.Tracert(targetPoint);
                        direction = board.Player.Position.GetDirectionTo(path.First());

                        board.Paths.Add(path);
                    }
                }
            }

            return $"{{\"command\": \"{direction.GetCommand()}\"}}";
        }

        public int GetScoreForMove(Board board, Player player, Point targetPoint, Direction direction)
        {
            var map = player.PossibleMaps[direction];
            var path = map.Tracert(targetPoint);

            if (path.Length == 0) return 0;

            var territoryMap = new Map2(board, board.Player, board.Player.Border.Select(e => e.Position).Where(p => !board.Player.Border[p].IsTerritory).ToArray());
            territoryMap.Check(board.Player.Line.First());

            var path2 = territoryMap.Tracert(targetPoint);
           
            var contur = path.ToList();
            contur.AddRange(board.Player.Line);
            contur.Add(board.Player.Position);

            var ticksToTarget = player.Map[player.Position[direction]].Weight + map[targetPoint].Weight;
            if (contur.Any(p => board.EnemiesMap[p] < ticksToTarget))
                return 0;

            contur.AddRange(path2);

            var s = GetPointsForPath(contur.ToArray()).ToArray();

            var score = 0;
            foreach (var point in s)
            {
                switch (board[point].Element)
                {
                    case Element.ME_TERRITORY: continue;
                    case Element.NONE: score += 1; break;
                    case Element.PLAYER_TERRITORY: score += 5; break;
                }
            }

            return score;
        }

        public IEnumerable<Point> GetPointsForPath(Point[] path)
        {
            var yMax = path.MaxSingle(p => p.Y).Y;
            var yMin = path.MinSingle(p => p.Y).Y;

            for (var y = yMin; y <= yMax; y++)
            {
                var xs = path.Where(p => p.Y == y).Select(p => p.X).Distinct().OrderBy(x => x).ToArray();

                var xMin = xs.Min();
                var xMax = xs.Max();

                for (var x = xMin; x <= xMax; x++)
                {
                    yield return new Point(x, y);
                }
//
//                if (xs.Length % 2 != 0)
//                    yield break;
//
//                for (var i = 0; i < xs.Length / 2; i++)
//                {
//                    var x0 = i * 2;
//                    var x1 = x0 + 1;
//
//                    for (var x = xs[x0]; x <= xs[x1]; x++)
//                    {
//                        yield return new Point(x, y);
//                    }
//                }
            }
        }

        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);
    }
}