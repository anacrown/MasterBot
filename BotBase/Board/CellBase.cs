using System;
using System.Linq;

namespace BotBase.Board
{
    public class CellBase
    {
        public string C { get; }

        public Board<CellBase> Board { get; }

        public Point Pos { get; set; }

        public int X => this.Pos.X;

        public int Y => this.Pos.Y;

        public CellBase(Point position)
        {
            this.Pos = position;
        }

        public CellBase(string c, Point position, Board<CellBase> board)
            : this(position)
        {
            this.C = c;
            this.Board = board;
        }

        public CellBase this[Direction direction] => this.Board[this.Pos[direction]];

        public CellBase[] GetCrossVicinity()
        {
            return this.Pos.GetCrossVicinity(this.Board.Size).Select<Point, CellBase>((Func<Point, CellBase>)(t => this.Board[t])).ToArray<CellBase>();
        }
    }
}