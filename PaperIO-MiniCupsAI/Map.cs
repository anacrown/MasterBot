using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Point = CodenjoyBot.Board.Point;

namespace PaperIO_MiniCupsAI
{
    public class Map : Matrix<MapEntry>
    {
        public Map(Size size, params Point[] checkedPoints)
            : base(size)
        {
            foreach (Point checkedPoint in checkedPoints)
                this[checkedPoint].BChecked = true;
        }

        public void Check(Point point)
        {
            List<Point> pointList = new List<Point>();
            pointList.AddRange(check(point));
            do
            {
                Point[] array = pointList.ToArray();
                pointList.Clear();
                foreach (Point point1 in array)
                    pointList.AddRange(check(point1));
            }
            while (pointList.Count > 0);
        }

        public IEnumerable<Point> Tracert(
            Point startPoint,
            Point endPoint)
        {
            for (Point prev = startPoint.GetCrossVicinity(Size).Select<Point, ValueTuple<Point, int>>(p => new ValueTuple<Point, int>(p, this[p].Weight)).Aggregate<ValueTuple<Point, int>>((i1, i2) =>
                {
                    if (i1.Item2 >= i2.Item2)
                        return i2;
                    return i1;
                }).Item1; prev != endPoint; prev = startPoint.GetCrossVicinity(Size).Select<Point, ValueTuple<Point, int>>(p => new ValueTuple<Point, int>(p, this[p].Weight)).Aggregate<ValueTuple<Point, int>>((i1, i2) =>
                {
                    if (i1.Item2 >= i2.Item2)
                        return i2;
                    return i1;
                }).Item1)
                yield return prev;
        }

        private IEnumerable<Point> check(Point point)
        {
            this[point].BChecked = true;
            Point[] array = point.GetCrossVicinity(Size).Where<Point>(n =>
            {
                if (!this[n].BChecked)
                    return !this[n].BWatched;
                return false;
            }).ToArray<Point>();
            foreach (Point index in array)
            {
                this[index].BWatched = true;
                this[index].Weight = this[point].Weight + 1;
            }
            return array;
        }
    }
}