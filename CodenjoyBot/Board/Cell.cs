using System;

namespace CodenjoyBot.Board
{
    public class Cell
    {
        public char C { get; }
        public CodenjoyBot.Board.Board Board { get; }

        public Point Pos { get; }
        public int X => Pos.X;
        public int Y => Pos.Y;

        public Cell(char c, Point position, CodenjoyBot.Board.Board board)
        {
            C = c;
            Board = board;
            Pos = position;
        }
    }
}