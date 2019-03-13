namespace CodenjoyBot.Board
{
    public class Cell
    {
        private readonly char _c;

        public CodenjoyBot.Board.Board Board { get; }

        public Point Pos { get; }
        public int X => Pos.X;
        public int Y => Pos.Y;

        public Cell(char c, Point position, CodenjoyBot.Board.Board board)
        {
            _c = c;
            Board = board;
            Pos = position;
        }
    }
}