using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotBase;
using BotBase.Board;

namespace SpaceRaceStrategy.AISolver
{
    public class Board : Board<Cell>
    {
        public Board(string instanceName, DateTime startTime, DataFrame frame) : base(instanceName, startTime, frame)
        {
            var size = (int) Math.Sqrt(frame.Board.Length);
            Size = new Size(size, size);

            Cells = new Cell[Size.Width * Size.Height];
            
            for (var x = 0; x < Size.Width; ++x)
            for (var y = 0; y < Size.Height; ++y)
                Cells[x + y * size] = new Cell(frame.Board[x + y * size].ToString(), new Point(x, y), this);
        }
    }
}
