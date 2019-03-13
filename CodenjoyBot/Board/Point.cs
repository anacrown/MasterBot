using System.Collections.Generic;
using System.Linq;

namespace CodenjoyBot.Board
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

        public IEnumerable<Point> GetCrossVicinity(int size) =>
            from dp in Neighbor.Values
            let v = this + dp
            where v.OnBoard(size)
            select v;

        public Direction GetDirectionTo(Point p)
        {
            if (X == p.X)
            {
                return Y < p.Y ? Direction.Down : Direction.Up;
            }

            if (Y == p.Y)
            {
                return X < p.X ? Direction.Right : Direction.Left;
            }

            return Direction.Unknown;
        }

        public Point this[Direction direction] => this + Neighbor[direction];

        public IEnumerable<Point> GetLine(Direction direction, int size, int deep = -1)
        {
            var point = this[direction];

            while (point.OnBoard(size))
            {
                if (deep == 0) yield break;

                yield return point;
                point = point[direction];
                deep--;
            }
        }

        public bool OnBoard(int size) => this >= 0 && this < size;

        public bool IsDiagonal(Point p) => DiagonalNeighbors.Select(t => this + t).Contains(p);

        public static Point Empty => new Point(0, 0);

        private static readonly Dictionary<Direction, Point> Neighbor =
            new Dictionary<Direction, Point>()
            {
                {Direction.Up, new Point(0, -1)},
                {Direction.Right, new Point(1, 0)},
                {Direction.Down, new Point(0, 1)},
                {Direction.Left, new Point(-1, 0)}
            };

        private static readonly Point[] DiagonalNeighbors = new Point[]
            {new Point(-1, -1), new Point(-1, 1), new Point(1, 1), new Point(1, -1),};

        public static Point operator +(Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);
        public static Point operator -(Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y);
        public static bool operator ==(Point p1, Point p2) => p1?.X == p2?.X && p1?.Y == p2?.Y;
        public static bool operator !=(Point p1, Point p2) => p1?.X != p2?.X || p1?.Y != p2?.Y;

        public static bool operator <(Point p, int i) => p.X < i && p.Y < i;
        public static bool operator >(Point p, int i) => p.X > i && p.Y > i;
        public static bool operator >=(Point p, int i) => p.X >= i && p.Y >= i;
        public static bool operator <=(Point p, int i) => p.X <= i && p.Y <= i;

        public override string ToString() => $"({X}:{Y})";
        public override bool Equals(object obj) => obj is Point point ? point == this : base.Equals(obj);

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}