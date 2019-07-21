using System;
using System.Linq;

namespace CodenjoyBot.Board
{
    public class Cell
    {
        public string C { get; }

        public CodenjoyBot.Board.Board<Cell> Board { get; }

        public Point Pos { get; set; }

        public int X
        {
            get
            {
                return this.Pos.X;
            }
        }

        public int Y
        {
            get
            {
                return this.Pos.Y;
            }
        }

        public Cell(Point position)
        {
            this.Pos = position;
        }

        public Cell(string c, Point position, CodenjoyBot.Board.Board<Cell> board)
            : this(position)
        {
            this.C = c;
            this.Board = board;
        }

        public Cell this[Direction direction]
        {
            get
            {
                return this.Board[this.Pos[direction]];
            }
        }

        public Cell[] GetCrossVicinity()
        {
            return this.Pos.GetCrossVicinity(this.Board.Size).Select<Point, Cell>((Func<Point, Cell>)(t => this.Board[t])).ToArray<Cell>();
        }
    }
}