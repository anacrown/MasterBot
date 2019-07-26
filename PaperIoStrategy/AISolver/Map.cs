using System.Collections.Generic;
using System.Linq;
using BotBase.Board;
using Point = BotBase.Board.Point;

namespace PaperIoStrategy.AISolver
{
    public class Map : Matrix<MapEntry>
    {
        public Point CheckPoint { get; private set; }

        public Map(Size size, params Point[] checkedPoints) : base(size)
        {
            foreach (var checkedPoint in checkedPoints)
            {
                this[checkedPoint].Weight = -1;
                this[checkedPoint].BChecked = true;
            }

            for (var i = 0; i < Size.Width; i++)
            for (var j = 0; j < Size.Height; j++)
                this[i, j].Position = new Point(i, j);
        }

        public void Check(Point checkPoint)
        {
            this[CheckPoint = checkPoint].Weight = 0;

            var pointList = new List<Point>();
            pointList.AddRange(check(CheckPoint));
            do
            {
                var array = pointList.ToArray();
                pointList.Clear();

                foreach (var point in array)
                    pointList.AddRange(check(point));
            }
            while (pointList.Count > 0);
        }

        public IEnumerable<Point> Tracert(Point point)
        {
//            if (fPoint == null || fPoint == CheckPoint) return null;
//
//            var point = fPoint;
//            var points = new List<Point>() { point };
//        
//            while (point != null)
//            {
//                var cross = point.GetCrossVicinity(Size).ToArray();
//
//                var min = cross.Select(t => this[t].Weight).Where(t => t >= 0).Min();
//                if (min == 0)
//                {
//                    if (cross.Contains(CheckPoint)) break;
//                    return null;
//                }
//
//                points.Add(point = cross.FirstOrDefault(t => this[t].Weight == min));
//            }
//
//            points.Reverse();
//            return points.ToArray();

            var entry = this[point];

            while (entry != null && entry.Weight > 0)
            {
                yield return entry.Position;
                entry = entry.Position.GetCrossVicinity(Size).Select(p => this[p]).FirstOrDefault(e => e.Weight == entry.Weight - 1);
            }
        }

        private IEnumerable<Point> check(Point point)
        {
            this[point].BChecked = true;
            var array = point.GetCrossVicinity(Size).Where(n => !this[n].BChecked && !this[n].BWatched).ToArray();

            foreach (var index in array)
            {
                this[index].BWatched = true;
                this[index].Weight = this[point].Weight + 1;
            }
            return array;
        }
    }
}