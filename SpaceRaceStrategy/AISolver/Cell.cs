using BotBase.Board;

namespace SpaceRaceStrategy.AISolver
{
    public class Cell : CellBase
    {
        public new Board Board { get; set; }
        public Element Element { get; set; }

        public Cell(string c, Point position, Board board) : base(c, position, null)
        {
            Board = board;
            Element = this.GetElement();
        }
    }
}
