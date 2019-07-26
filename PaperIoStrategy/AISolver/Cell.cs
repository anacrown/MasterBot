using System.Linq;
using BotBase.Board;

namespace PaperIoStrategy.AISolver
{
    public class Cell : BotBase.Board.Cell
    {
        public Element Element { get; set; }

        public Board Board { get; set; }

        public Cell(Point position, Board board, Element element = Element.NONE)
            : base(position)
        {
            Element = element;
            Board = board;
        }

        public Cell this[Direction direction] => Board[Pos[direction]];

        public Cell[] GetCrossVicinity() => Pos.GetCrossVicinity(Board.Size).Select<Point, Cell>(t => Board[t]).ToArray<Cell>();
    }
}