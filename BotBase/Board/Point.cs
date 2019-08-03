using System;
using System.Collections.Generic;
using System.Linq;

namespace BotBase.Board
{
    public class Point
    {
        private static readonly Point[] DiagonalNeighbors = new Point[4]
        {
            new Point(-1, -1),
            new Point(-1, 1),
            new Point(1, 1),
            new Point(1, -1)
        };

        public int X { get; set; }

        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IEnumerable<Point> GetCrossVicinity(Size size)
        {
            return Neighbor.Values.Select(dp => new
            {
                dp = dp,
                v = this + dp
            }).Where(p => p.v.OnBoard(size)).Select(p => p.v);
        }

        public Direction GetDirectionTo(Point p)
        {
            var dP = p - this;
            var neighborPair = Neighbor.SingleOrDefault(pair => pair.Value == dP);

            return neighborPair.Key;
        }

        public Point this[Direction direction] => this + Neighbor[direction];

        public IEnumerable<Point> GetLine(Direction direction, Size size, int deep = -1)
        {
            for (var point = this[direction]; point.OnBoard(size) && deep != 0; --deep)
            {
                yield return point;
                point = point[direction];
            }
        }

        public bool OnBoard(Size size)
        {
            if (this >= 0)
                return this < size;
            return false;
        }

        public bool OnBoard(int Size) => OnBoard(new Size(Size, Size));

        public bool IsDiagonal(Point p) => DiagonalNeighbors.Select(t => this + t).Contains(p);

        public static Point Empty => new Point(0, 0);

        public bool IsEmpty => X == 0 && Y == 0;

        public static Dictionary<Direction, Point> Neighbor { get; set; } = new Dictionary<Direction, Point>()
        {
            {Direction.Up, new Point(0, 1)},
            {Direction.Right, new Point(1, 0)},
            {Direction.Down, new Point(0, -1)},
            {Direction.Left, new Point(-1, 0)}
        };

        public static Point operator +(Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);

        public static Point operator -(Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y);

        public static Point operator +(Point p1, int i) => new Point(p1.X + i, p1.Y + i);

        public static Point operator -(Point p1, int i) => new Point(p1.X - i, p1.Y - i);

        public static Point operator *(Point p1, int i) => new Point(p1.X * i, p1.Y * i);

        public static Point operator /(Point p1, int i) => new Point(p1.X / i, p1.Y / i);

        public static Point operator %(Point p1, Point p2) => new Point(p1.X % p2.X, p1.Y % p2.Y);

        public static Point operator %(Point p1, int i) => new Point(p1.X % i, p1.Y % i);

        public static bool operator ==(Point p, int i) => p.X == i && p.Y == i;

        public static bool operator !=(Point p, int i) => !(p == i);

        public double Abs()
        {
            if (X == 0) return Y;
            if (Y == 0) return X;
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public static bool operator ==(Point p1, Point p2)
        {
            var nullable1 = (object)p1 != null ? p1.X : new int?();
            var nullable2 = (object)p2 != null ? p2.X : new int?();
            if (!(nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue))
                return false;
            nullable2 = (object)p1 != null ? p1.Y : new int?();
            nullable1 = (object)p2 != null ? p2.Y : new int?();
            return nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() & nullable2.HasValue == nullable1.HasValue;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            var nullable1 = (object)p1 != null ? p1.X : new int?();
            var nullable2 = (object)p2 != null ? p2.X : new int?();
            if (!(nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue))
                return true;
            nullable2 = (object)p1 != null ? p1.Y : new int?();
            nullable1 = (object)p2 != null ? p2.Y : new int?();
            return !(nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() & nullable2.HasValue == nullable1.HasValue);
        }

        public static bool operator <(Point p, Size size)
        {
            if (p.X < size.Width)
                return p.Y < size.Height;
            return false;
        }

        public static bool operator >(Point p, Size size)
        {
            if (p.X > size.Width)
                return p.Y > size.Height;
            return false;
        }

        public static bool operator >=(Point p, Size size)
        {
            if (p.X >= size.Width)
                return p.Y >= size.Height;
            return false;
        }

        public static bool operator <=(Point p, Size size)
        {
            if (p.X <= size.Width)
                return p.Y <= size.Height;
            return false;
        }

        public static bool operator <(Point p, int i)
        {
            if (p.X < i)
                return p.Y < i;
            return false;
        }

        public static bool operator >(Point p, int i)
        {
            if (p.X > i)
                return p.Y > i;
            return false;
        }

        public static bool operator >=(Point p, int i)
        {
            if (p.X >= i)
                return p.Y >= i;
            return false;
        }

        public static bool operator <=(Point p, int i)
        {
            if (p.X <= i)
                return p.Y <= i;
            return false;
        }

        public override string ToString()
        {
            return string.Format("({0}:{1})", X, Y);
        }

        protected bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return X * 397 ^ Y;
        }
    }
}