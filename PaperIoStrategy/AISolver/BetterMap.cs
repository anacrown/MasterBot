using System;
using System.Collections.Generic;
using System.Linq;
using BotBase.Board;

namespace PaperIoStrategy.AISolver
{
    public class BetterMap : Matrix<MapEntry>
    {
        public Board Board { get; }

        public BetterMap(Board board) : base(board.Size)
        {
            Board = board;

            for (var i = 0; i < Size.Width; i++)
            for (var j = 0; j < Size.Height; j++)
                this[i, j].Position = new Point(i, j);

            if (board.Enemies == null || !board.Enemies.Any()) return;

            foreach (var point in board.Enemies.SelectMany(e => e.Territory))
            {
                Check(point);

                foreach (var e in this) e.BChecked = e.BWatched = false;
            }
        }

        void Check(Point checkPoint)
        {
            this[checkPoint].Weight += 10;

            var pointList = new List<Point>() { checkPoint };

            do
            {
                var array = pointList.ToArray();
                pointList.Clear();

                foreach (var p in array)
                    pointList.AddRange(check(p));
            }
            while (pointList.Count > 0);
        }

        private IEnumerable<Point> check(Point point)
        {
            this[point].BChecked = true;
            var array = point.GetCrossVicinity(Size).Where(n => !this[n].BChecked && !this[n].BWatched).ToArray();

            foreach (var index in array)
            {
                this[index].BWatched = true;
                this[index].Weight = Math.Max(this[index].Weight, this[point].Weight - 1);
            }

            return array;
        }
    }
}
