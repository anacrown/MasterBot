using System;
using System.Collections.Generic;
using System.Linq;
using BotBase.Board;

namespace PaperIoStrategy.AISolver
{
    public class BetterMap : Matrix<MapEntry>
    {
        public Board Board { get; }
        public Player Player { get; }

        public BetterMap(Board board, Player player) : base(board.Size)
        {
            Board = board;
            Player = player;

            for (var i = 0; i < Size.Width; i++)
                for (var j = 0; j < Size.Height; j++)
                    this[i, j].Position = new Point(i, j);

            if (Player.Line.Any())
                Check(Player.Line.ToArray());
        }

        void Check(Point[] checkPoints)
        {
            foreach (var checkPoint in checkPoints)
            {
                this[checkPoint].BChecked = true;
                this[checkPoint].BWatched = true;
                this[checkPoint].Weight = 0;
            }

            var pointList = checkPoints.ToList();

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
                this[index].Weight += this[point].Weight + 1;
            }

            return array;
        }
    }
}
