using System;
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

        internal void Check(Point checkPoint, int width, int startWeight, params SpeedSnapshot[] speedSnapshots)
        {
            this[CheckPoint = checkPoint].Weight = startWeight;

            var currentSnapshotIndex = 0;
            var speedSnapshot = speedSnapshots.First();

            var ticks = width / speedSnapshot.Speed;
            var pointList = new List<Point>() { CheckPoint };
            
            do
            {
                var array = pointList.ToArray();
                pointList.Clear();

                speedSnapshot.Pixels -= width;

                if (speedSnapshot.Pixels <= 0)
                {
                    speedSnapshot = speedSnapshots[++currentSnapshotIndex];
                    ticks = width / speedSnapshot.Speed;
                }

                foreach (var point in array)
                    pointList.AddRange(check(point, ticks));
            }
            while (pointList.Count > 0);
        }

        private IEnumerable<Point> check(Point point, int ticks)
        {
            this[point].BChecked = true;
            var array = point.GetCrossVicinity(Size).Where(n => !this[n].BChecked && !this[n].BWatched).ToArray();

            foreach (var index in array)
            {
                this[index].BWatched = true;
                this[index].Weight = this[point].Weight + ticks;
            }
            return array;
        }

        public Point[] Tracert(Point point) => tracert_forward(point).Reverse().ToArray();

        private IEnumerable<Point> tracert_forward(Point point)
        {
            var entry = this[point];

            while (entry != null && entry.Weight > 0)
            {
                yield return entry.Position;
                entry = entry.Position.GetCrossVicinity(Size).Select(p => this[p]).MinSingle(e => e.Weight);
            }
        }
    }
}