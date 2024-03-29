﻿using System.Collections.Generic;
using System.Linq;
using BotBase.Board;

namespace PaperIoStrategy.AISolver
{
    public class BorderCell
    {
        public Point Position { get; set; }

        public bool IsBoundary { get; set; }
        public bool IsTerritory { get; set; }
        public Direction[] OutDirections { get; set; }
    }

    public class Border : Matrix<BorderCell>
    {
        public Board Board { get; }

        public Border(Board board, IEnumerable<Point> territory) : base(board.Size)
        {
            Board = board;

            foreach (var point in territory) this[point].IsTerritory = true;

            for (var i = 0; i < Size.Width; i++)
                for (var j = 0; j < Size.Height; j++)
                {
                    this[i, j].Position = new Point(i, j);

                    if (!this[i, j].IsTerritory) continue;
                    
                    this[i, j].OutDirections = (from d in Point.CrossNeighbors.Keys
                                                let p = this[i, j].Position[d]
                                                where p.OnBoard(Size) && !this[p].IsTerritory
                                                select d).ToArray();

                    this[i, j].IsBoundary = this[i, j].OutDirections.Length > 0;
                }
        }

        //public IEnumerable<Point> GetAlongPath(Point point)
        //{
        //    if (!this[point].IsBoundary) yield break;
        //
        //    var p = new Point(point);
        //
        //    yield return p;
        //
        //    this[p].OutDirection = Point.Neighbors.Keys.Last(t => !this[point[t]].IsTerritory);
        //
        //    do
        //    {
        //        var d = this[p].OutDirection;
        //        while (!this[p[d]].IsTerritory) d = d.Clockwise();
        //        d = d.CounterClockwise();
        //
        //        var nonePoint = p[d];
        //        d = d.Invert().CounterClockwise();
        //        while (!this[nonePoint[d]].IsBoundary) d = d.CounterClockwise();
        //
        //        var t = nonePoint[d];
        //        if (!this[t].IsBoundary) yield break;
        //        while (t.OnBoard(_board.Size) && this[t].IsTerritory)
        //        {
        //            p = t;
        //            this[p].OutDirection = d.Invert();
        //            yield return p;
        //
        //            t = nonePoint[d = d.CounterClockwise()];
        //            if (t == point) yield break;
        //        }
        //    } while (p != point);
        //}
    }
}