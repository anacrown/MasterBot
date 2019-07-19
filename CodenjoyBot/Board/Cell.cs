﻿using System.Linq;

namespace CodenjoyBot.Board
{
    public class Cell
    {
        public string C { get; }
        public CodenjoyBot.Board.Board Board { get; }

        public Point Pos { get; }
        public int X => Pos.X;
        public int Y => Pos.Y;

        public Cell(string c, Point position, CodenjoyBot.Board.Board board)
        {
            C = c;
            Board = board;
            Pos = position;
        }

        public Cell[] GetCrossVicinity() => Pos.GetCrossVicinity(Board.Size).Select(t => Board[t]).ToArray();
    }
}