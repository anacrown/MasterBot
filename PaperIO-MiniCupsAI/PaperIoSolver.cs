using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;
using CodenjoyBot.Interfaces;
using Newtonsoft.Json.Linq;
using PaperIO_MiniCupsAI.Controls;
using Point = CodenjoyBot.Board.Point;
using Size = System.Drawing.Size;

namespace PaperIO_MiniCupsAI
{
    [Serializable]
    public class PaperIoSolver : ISolver
    {
        private int _speed;
        private int _width;
        private int _boardSizeX;
        private int _boardSizeY;

        public UIElement Control { get; }

        public UIElement DebugControl { get; }

        public event EventHandler<Board> BoardChanged;

        public event EventHandler<LogRecord> LogDataReceived;

        public PaperIoSolver()
        {
            Control = new PaperIoSolverControl(this);
            DebugControl = new PaperIoSolverDebugControl(this);
            Point.Neighbor = new Dictionary<Direction, Point>()
      {
        {
          Direction.Up,
          new Point(0, 1)
        },
        {
          Direction.Right,
          new Point(1, 0)
        },
        {
          Direction.Down,
          new Point(0, -1)
        },
        {
          Direction.Left,
          new Point(-1, 0)
        }
      };
        }

        public PaperIoSolver(SerializationInfo info, StreamingContext context)
          : this()
        {
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        public void Initialize()
        {
        }

        public string Answer(string instanceName, DateTime startTime, DataFrame frame)
        {
            Board board = LoadData(instanceName, startTime, frame);
            SolverCommand solverCommand = SolverCommand.Empty;
            if (board.MeCell != null)
            {
                board.Where<Cell>(c =>
                {
                    if (c.Element != Element.ME)
                        return c.Element == Element.ME_LINE;
                    return true;
                }).Select<Cell, Point>(c => c.Pos).Select<Point, ValueTuple<Point, int>>(p => new ValueTuple<Point, int>(p, board.OppWeights.Values.Select<Map, int>(map => map[p].Weight).Min())).Aggregate<ValueTuple<Point, int>>((i1, i2) =>
                {
                    if (i1.Item2 >= i2.Item2)
                        return i2;
                    return i1;
                });
                ValueTuple<Point, int> valueTuple = board.Where<Cell>(c => c.Element == Element.ME_TERRITORY).Select<Cell, Point>(c => c.Pos).Select<Point, ValueTuple<Point, int>>(p => new ValueTuple<Point, int>(p, board.MeWeight[p].Weight)).Aggregate<ValueTuple<Point, int>>((i1, i2) =>
                {
                    if (i1.Item2 >= i2.Item2)
                        return i2;
                    return i1;
                });
                board.PathToHome = board.MeWeight.Tracert(valueTuple.Item1, board.MeCell.Pos).ToArray<Point>();
                Point p1 = board.MeCell.Pos.GetCrossVicinity(board.Size).Where<Point>(p =>
                {
                    if (p.X != 0 && (p.X != board.Size.Width - 1 && p.Y != 0))
                        return p.Y != board.Size.Height - 1;
                    return false;
                }).Select<Point, ValueTuple<Point, int>>(p => new ValueTuple<Point, int>(p, board.OppWeights.Values.Select<Map, int>(map => map[p].Weight).Min())).Aggregate<ValueTuple<Point, int>>((i1, i2) =>
                {
                    if (i1.Item2 <= i2.Item2)
                        return i2;
                    return i1;
                }).Item1;
                solverCommand = board.MeCell.Pos.GetDirectionTo(p1).GetCommand();
            }

            LogDataReceived?.Invoke(this, new LogRecord(frame, $"command: {solverCommand}"));
            OnBoardChanged(board);
            return $"{{\"command\": \"{solverCommand}\"}}";
        }

        private Board LoadData(
          string instanceName,
          DateTime startTime,
          DataFrame frame)
        {
            JObject jobject = JObject.Parse(frame.Board);
            JToken jtoken1 = jobject["type"];
            Board source;
            if ((jtoken1 != null ? jtoken1.Value<string>() : null) == "start_game")
            {
                _speed = jobject["params"]["speed"].Value<int>();
                _width = jobject["params"]["width"].Value<int>();
                _boardSizeX = jobject["params"]["x_cells_count"].Value<int>();
                _boardSizeY = jobject["params"]["y_cells_count"].Value<int>();
                source = new Board(instanceName, startTime, frame, new Size(_boardSizeX, _boardSizeY));
            }
            else
            {
                JToken jtoken2 = jobject["type"];
                if (!((jtoken2 != null ? jtoken2.Value<string>() : null) == "tick"))
                    throw new Exception("Invalide input data");
                source = new Board(instanceName, startTime, frame, new Size(_boardSizeX, _boardSizeY));
                foreach (JProperty child in jobject["params"]["players"].Children<JProperty>())
                {
                    string name = child.Name;
                    JToken first = child.First;
                    Direction direction = first["direction"].Value<string>().ToDirection();
                    Point point = GetPoint(first["position"].Values<int>().ToArray<int>(), _width, source.Size);
                    Cell playerCell = source[point];
                    playerCell.Direction = direction;
                    foreach (int[] numArray in first["lines"].Children().Select<JToken, int[]>(t => t.Values<int>().ToArray<int>()))
                    {
                        Cell cell = source[GetPoint(numArray, _width, source.Size)];
                        cell.PlayerName = name;
                        cell.Element = name == "i" ? Element.ME_LINE : Element.PLAYER_LINE;
                    }
                    foreach (int[] numArray in first["territory"].Children().Select<JToken, int[]>(t => t.Values<int>().ToArray<int>()))
                    {
                        Cell cell = source[GetPoint(numArray, _width, source.Size)];
                        cell.PlayerName = name;
                        cell.Element = name == "i" ? Element.ME_TERRITORY : Element.PLAYER_TERRITORY;
                    }
                    playerCell.PlayerName = name;
                    playerCell.Element = name == "i" ? Element.ME : Element.PLAYER;
                    if (playerCell.Element == Element.ME)
                    {
                        source.MeCell = playerCell;
                        source.MeWeight = new Map(source.Size, source.Where<Cell>(c => c.Element == Element.ME_LINE).Select<Cell, Point>(c => c.Pos).ToArray<Point>());
                        source.MeWeight.Check(source.MeCell.Pos);
                    }
                    else
                    {
                        Map map = new Map(source.Size, source.Where<Cell>(c =>
                        {
                            if (c.Element == Element.PLAYER_LINE)
                                return c.PlayerName == playerCell.PlayerName;
                            return false;
                        }).Select<Cell, Point>(c => c.Pos).ToArray<Point>());
                        map.Check(playerCell.Pos);
                        source.OppWeights.Add(playerCell.PlayerName, map);
                    }
                }
            }
            return source;
        }

        private Point GetPoint(IReadOnlyList<int> arr, int cellWidth, Size size)
        {
            return new Point(arr[0] / cellWidth, arr[1] / cellWidth);
        }

        protected virtual void OnBoardChanged(Board board)
        {
            EventHandler<Board> boardChanged = BoardChanged;
            if (boardChanged == null)
                return;
            boardChanged(this, board);
        }
    }
}
