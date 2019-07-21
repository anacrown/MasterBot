using System.Linq;
using CodenjoyBot.Board;

namespace PaperIO_MiniCupsAI
{
    public class Cell : CodenjoyBot.Board.Cell
    {
        public Element Element { get; set; }

        public Board Board { get; set; }

        public string PlayerName { get; set; }

        public Direction Direction { get; set; } = Direction.Unknown;

        public Cell(Point position, Board board, Element element = Element.NONE)
            : base(position)
        {
            Element = element;
            Board = board;
        }

        public Cell this[Direction direction]
        {
            get
            {
                return Board[Pos[direction]];
            }
        }

        public Cell[] GetCrossVicinity()
        {
            return Pos.GetCrossVicinity(Board.Size).Select<Point, Cell>(t => Board[t]).ToArray<Cell>();
        }
    }
}