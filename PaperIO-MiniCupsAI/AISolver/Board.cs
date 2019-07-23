using System;
using System.Collections.Generic;
using System.Drawing;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;
using Point = CodenjoyBot.Board.Point;

namespace PaperIO_MiniCupsAI
{
    public class Board : Board<Cell>
    {
        public BoardType BoardType { get; set; }

        public Cell MeCell { get; set; }

        public Map MeWeight { get; set; }

        public Dictionary<string, Map> OppWeights { get; set; } = new Dictionary<string, Map>();

        public Point[] PathToHome { get; set; }

        public Board(string instanceName, DateTime startTime, DataFrame frame, Size size): base(instanceName, startTime, frame)
        {
            Size = size;

            Cells = new Cell[size.Width * size.Height];
            for (var index = 0; index < Cells.Length; ++index)
                Cells[index] = new Cell(Point.Empty, this);

            for (var x = 0; x < size.Width; ++x)
            for (var y = 0; y < size.Height; ++y)
                this[x, y].Pos = new Point(x, y);
        }
    }

    public enum BoardType
    {
        StartGame, Tick, EndGame
    }
}