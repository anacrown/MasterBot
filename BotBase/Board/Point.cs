using System;
using System.Collections.Generic;
using System.Linq;

namespace BotBase.Board
{
    public class Point
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public IEnumerable<Point> GetCrossVicinity(Size size)
        {
            return CrossNeighbors.Values.Select(dp => new
            {
                dp = dp,
                v = this + dp
            }).Where(p => p.v.OnBoard(size)).Select(p => p.v);
        }

        public Direction GetDirectionTo(Point p)
        {
            var dP = p - this;
            var neighborPair = Neighbors.SingleOrDefault(pair => pair.Value == dP);

            return neighborPair.Key;
        }

        public Point this[Direction direction] => this + Neighbors[direction];

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

        public bool IsDiagonal(Point p) => DiagonalNeighbors.Select(t => this + t.Value).Contains(p);

        public static Point Empty => new Point(0, 0);

        public bool IsEmpty => X == 0 && Y == 0;

        public static Dictionary<Direction, Point> CrossNeighbors { get; set; } = new Dictionary<Direction, Point>()
        {
            {Direction.Up, new Point(0, 1)},
            {Direction.Right, new Point(1, 0)},
            {Direction.Down, new Point(0, -1)},
            {Direction.Left, new Point(-1, 0)}
        };

        public static Dictionary<Direction, Point> DiagonalNeighbors { get; set; } = new Dictionary<Direction, Point>()
        {
            {Direction.UpRight, new Point(1, 1)},
            {Direction.DownRight, new Point(1, -1)},
            {Direction.DownLeft, new Point(-1, -1)},
            {Direction.UpLeft, new Point(-1, 1)}
        };

        public static Dictionary<Direction, Point> Neighbors { get; set; } = new Dictionary<Direction, Point>()
        {
            {Direction.Up, new Point(0, 1)},
            {Direction.UpRight, new Point(1, 1)},
            {Direction.Right, new Point(1, 0)},
            {Direction.DownRight, new Point(1, -1)},
            {Direction.Down, new Point(0, -1)},
            {Direction.DownLeft, new Point(-1, -1)},
            {Direction.Left, new Point(-1, 0)},
            {Direction.UpLeft, new Point(-1, 1)}
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

        public static bool operator <(Point p, Size size) => p.X < size.Width && p.Y < size.Height;

        public static bool operator >(Point p, Size size) => p.X > size.Width && p.Y > size.Height;

        public static bool operator >=(Point p, Size size) => p.X >= size.Width && p.Y >= size.Height;

        public static bool operator <=(Point p, Size size) => p.X <= size.Width && p.Y <= size.Height;

        public static bool operator <(Point p, int i) => p.X < i && p.Y < i;

        public static bool operator >(Point p, int i) => p.X > i && p.Y > i;

        public static bool operator >=(Point p, int i) => p.X >= i && p.Y >= i;

        public static bool operator <=(Point p, int i) => p.X <= i && p.Y <= i;

        public int Abs()
        {
            if (X == 0) return Y > 0 ? Y : -Y;
            if (Y == 0) return X > 0 ? X : -X;
            return -1;
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

        public override string ToString() => $"({X}:{Y})";

        protected bool Equals(Point other) => X == other.X && Y == other.Y;

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

        public override int GetHashCode() => X * 397 ^ Y;
    }
}