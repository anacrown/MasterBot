using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;
using CodenjoyBot.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaperIO_MiniCupsAI.ActionSolvers;
using PaperIO_MiniCupsAI.Controls;
using PaperIO_MiniCupsAI.DataContract;
using Point = CodenjoyBot.Board.Point;
using Size = System.Drawing.Size;

namespace PaperIO_MiniCupsAI
{
    [Serializable]
    public class PaperIoSolver : ISolver
    {
        private JPacket _startInfo;

        public UIElement Control { get; }

        public UIElement DebugControl { get; }

        public event EventHandler<Board> BoardChanged;
        public event EventHandler<LogRecord> LogDataReceived;

        public PaperIoChecker PaperIoChecker { get; private set; } = new PaperIoChecker();
        public DefenseChecker DefenseChecker { get; private set; } = new DefenseChecker();

        public PaperIoSolver()
        {
            Control = new PaperIoSolverControl(this);
            DebugControl = new PaperIoSolverDebugControl(this);
            Point.Neighbor = new Dictionary<Direction, Point>
            {
                {Direction.Up, new Point(0, 1)},
                {Direction.Right, new Point(1, 0)},
                {Direction.Down, new Point(0, -1)},
                {Direction.Left, new Point(-1, 0)}
            };
        }

        public PaperIoSolver(SerializationInfo info, StreamingContext context) : this() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

        public void Initialize() { }

        public bool Answer(string instanceName, DateTime startTime, DataFrame frame, IDataProvider dataProvider, out string response)
        {
            response = string.Empty;
            var jPacket = JsonConvert.DeserializeObject<JPacket>(frame.Board);

            if (jPacket.PacketType == JPacketType.EndGame)
            {
                dataProvider.Stop();
                return false;
            }

            var board = new Board(instanceName, startTime, frame, jPacket.PacketType == JPacketType.StartGame ? _startInfo = jPacket : jPacket.Merge(_startInfo));
            OnBoardChanged(board);

            if (board.JPacket.PacketType != JPacketType.Tick) return false;

            response = GetResponse(board);

            return true;
        }

        private string GetResponse(Board board)
        {

            // из возможных для хода клеток выбираем ту, откуда можно безопасно вернуться домой

//            var directionBack = board.IPlayer.Direction.Invert();
//            var pointEntries = Point.Neighbor
//                .Where(pair => pair.Key != directionBack)
//                .Select(pair => board.IPlayer.Position[pair.Key])
//                .Where(point => point.OnBoard(board.Size))
//                .Select(point => (Point: point, EnemyW: board.Enemies.Select(enemy => enemy.Map[point]).Min()))
//                .OrderByDescending(entry => entry.EnemyW).ToArray();

//            var pointEntries = from pair in Point.Neighbor
//                where pair.Key != directionBack
//                let point = board.IPlayer.Position[pair.Key]
//                where point.OnBoard(board.Size)
//                let entry = (Point: point, EnemyW: board.Enemies.Select(enemy => enemy.Map[point]).Min())
//                orderby entry.EnemyW descending
//                select entry;



//            if (board.PathToHome != null && board.PathToHome.Any())
//            {
//                return $"{{\"command\": \"{board.IPlayer.Position.GetDirectionTo(board.PathToHome.First()).GetCommand()}\"}}";
//            }
//            else return string.Empty;

            var directions = Point.Neighbor.Keys.Where(d => PaperIoChecker.CanIGoTo(board, d) &&
                                                            DefenseChecker.CanIGoTo(board, d))
                .ToList();

            OnLogDataReceived(new LogRecord(board.Frame, $"Directions: {string.Join(" ", directions)}"));

            if (!directions.Any()) directions.Add(board.IPlayer.Direction);

            return $"{{\"command\": \"{directions.First().GetCommand()}\"}}";
        }

        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);
        protected virtual void OnBoardChanged(Board e) => BoardChanged?.Invoke(this, e);
    }
}
