using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleBot_SuperAI.BattleSolver
{
    public class Cross
    {
        public uint Time { get; }
        public BattleCell Centre { get; }
        public CrossCell[] Cells;

        public Cross(BattleCell centre, uint time, int skipFromCenter = 0)
        {
            Centre = centre;
            Time = time;
            Cells = GetCells(skipFromCenter).ToArray();
        }

        private IEnumerable<CrossCell> GetCells(int skip)
        {
            if (skip == 0)
                yield return new CrossCell(Centre, Centre, 0);

            for (var i = Centre.X - skip - 1; i >= 0; i--)
            {
                var cell = Centre.Board[i, Centre.Y];

                if (!cell.IsElapse)
                    break;

                var bulletTime = (int)Math.Ceiling((double)Math.Abs(Centre.X - i) / 2);

                yield return new CrossCell(Centre, cell, bulletTime);
            }

            for (var i = Centre.X + skip + 1; i < Centre.Board.Size; i++)
            {
                var cell = Centre.Board[i, Centre.Y];

                if (!cell.IsElapse)
                    break;

                var bulletTime = (int)Math.Ceiling((double)Math.Abs(Centre.X - i) / 2);

                yield return new CrossCell(Centre, cell, bulletTime);
            }

            for (var j = Centre.Y - skip - 1; j >= 0; j--)
            {
                var cell = Centre.Board[Centre.X, j];

                if (!cell.IsElapse)
                    break;

                var bulletTime = (int)Math.Ceiling((double)Math.Abs(Centre.Y - j) / 2);

                yield return new CrossCell(Centre, cell, bulletTime);
            }

            for (var j = Centre.Y + skip + 1; j < Centre.Board.Size; j++)
            {
                var cell = Centre.Board[Centre.X, j];

                if (!cell.IsElapse)
                    break;

                var bulletTime = (int)Math.Ceiling((double)Math.Abs(Centre.Y - j) / 2);

                yield return new CrossCell(Centre, cell, bulletTime);
            }
        }
    }
}