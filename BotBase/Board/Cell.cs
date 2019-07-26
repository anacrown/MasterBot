using System;
using System.Linq;

namespace BotBase.Board
{
    public class Cell
    {
        public string C { get; }

        public Board<Cell> Board { get; }

        public Point Pos { get; set; }

        public int X => this.Pos.X;

        public int Y => this.Pos.Y;

        public Cell(Point position)
        {
            this.Pos = position;
        }

        public Cell(string c, Point position, Board<Cell> board)
            : this(position)
        {
            this.C = c;
            this.Board = board;
        }

        public Cell this[Direction direction] => this.Board[this.Pos[direction]];

        public Cell[] GetCrossVicinity()
        {
            return this.Pos.GetCrossVicinity(this.Board.Size).Select<Point, Cell>((Func<Point, Cell>)(t => this.Board[t])).ToArray<Cell>();
        }
    }
}