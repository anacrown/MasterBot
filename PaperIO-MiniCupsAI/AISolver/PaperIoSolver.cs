﻿using System;
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

            if (string.IsNullOrEmpty(frame.Board))
            {
                dataProvider.Stop();
                return false;
            }

            var board = LoadData(instanceName, startTime, frame);
            if (board.BoardType != BoardType.Tick) return false;

            var commands = new string[4] { "left", "right", "up", "down" };
            var random = new Random();
            var index = random.Next(0, commands.Length);

            response = $"{{\"command\": \"{commands[index]}\"}}";
            return true;
        }

        private Board LoadData(string instanceName, DateTime startTime, DataFrame frame)
        {
            var jObject = JObject.Parse(frame.Board);
            var jToken1 = jObject["type"];
            Board board;
            if ((jToken1 != null ? jToken1.Value<string>() : null) == "start_game")
            {
                _speed = jObject["params"]["speed"].Value<int>();
                _width = jObject["params"]["width"].Value<int>();
                _boardSizeX = jObject["params"]["x_cells_count"].Value<int>();
                _boardSizeY = jObject["params"]["y_cells_count"].Value<int>();
                board = new Board(instanceName, startTime, frame, new Size(_boardSizeX, _boardSizeY));
                board.BoardType = BoardType.StartGame;
            }
            else
            {
                var jtoken2 = jObject["type"];
                if (!((jtoken2 != null ? jtoken2.Value<string>() : null) == "tick"))
                    throw new Exception("Invalide input data");
                board = new Board(instanceName, startTime, frame, new Size(_boardSizeX, _boardSizeY));
                board.BoardType = BoardType.Tick;
                foreach (var child in jObject["params"]["players"].Children<JProperty>())
                {
                    var name = child.Name;
                    var first = child.First;
                    var direction = first["direction"].Value<string>().ToDirection();
                    var point = GetPoint(first["position"].Values<int>().ToArray(), _width, board.Size);
                    var playerCell = board[point];
                    playerCell.Direction = direction;
                    foreach (var numArray in first["lines"].Children().Select(t => t.Values<int>().ToArray()))
                    {
                        var cell = board[GetPoint(numArray, _width, board.Size)];
                        cell.PlayerName = name;
                        cell.Element = name == "i" ? Element.ME_LINE : Element.PLAYER_LINE;
                    }
                    foreach (var numArray in first["territory"].Children().Select(t => t.Values<int>().ToArray()))
                    {
                        var cell = board[GetPoint(numArray, _width, board.Size)];
                        cell.PlayerName = name;
                        cell.Element = name == "i" ? Element.ME_TERRITORY : Element.PLAYER_TERRITORY;
                    }
                    playerCell.PlayerName = name;
                    playerCell.Element = name == "i" ? Element.ME : Element.PLAYER;
                    if (playerCell.Element == Element.ME)
                    {
                        board.MeCell = playerCell;
                        board.MeWeight = new Map(board.Size, board.Where(c => c.Element == Element.ME_LINE).Select(c => c.Pos).ToArray());
                        board.MeWeight.Check(board.MeCell.Pos);
                    }
                    else
                    {
                        var map = new Map(board.Size, board.Where(c =>
                        {
                            if (c.Element == Element.PLAYER_LINE)
                                return c.PlayerName == playerCell.PlayerName;
                            return false;
                        }).Select(c => c.Pos).ToArray());
                        map.Check(playerCell.Pos);
                        board.OppWeights.Add(playerCell.PlayerName, map);
                    }
                }
            }

            OnBoardChanged(board);
            return board;
        }

        private Point GetPoint(IReadOnlyList<int> arr, int cellWidth, Size size) => new Point(arr[0] / cellWidth, arr[1] / cellWidth);
        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);
        protected virtual void OnBoardChanged(Board e) => BoardChanged?.Invoke(this, e);
    }
}