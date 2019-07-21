using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CodenjoyBot.Board
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
            this.X = x;
            this.Y = y;
        }

        public IEnumerable<Point> GetCrossVicinity(Size size)
        {
            return Point.Neighbor.Values.Select(dp => new
            {
                dp = dp,
                v = this + dp
            }).Where(_param1 => _param1.v.OnBoard(size)).Select(_param1 => _param1.v);
        }

        public Direction GetDirectionTo(Point p)
        {
            if (this.X == p.X)
                return this.Y < p.Y ? Direction.Down : Direction.Up;
            if (this.Y == p.Y)
                return this.X < p.X ? Direction.Right : Direction.Left;
            return Direction.Unknown;
        }

        public Point this[Direction direction]
        {
            get
            {
                return this + Point.Neighbor[direction];
            }
        }

        public IEnumerable<Point> GetLine(Direction direction, Size size, int deep = -1)
        {
            for (Point point = this[direction]; point.OnBoard(size) && deep != 0; --deep)
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

        public bool IsDiagonal(Point p)
        {
            return ((IEnumerable<Point>)Point.DiagonalNeighbors).Select<Point, Point>((Func<Point, Point>)(t => this + t)).Contains<Point>(p);
        }

        public static Point Empty
        {
            get
            {
                return new Point(0, 0);
            }
        }

        public static Dictionary<Direction, Point> Neighbor { get; set; } = new Dictionary<Direction, Point>()
    {
      {
        Direction.Up,
        new Point(0, -1)
      },
      {
        Direction.Right,
        new Point(1, 0)
      },
      {
        Direction.Down,
        new Point(0, 1)
      },
      {
        Direction.Left,
        new Point(-1, 0)
      }
    };

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            int? nullable1 = (object)p1 != null ? new int?(p1.X) : new int?();
            int? nullable2 = (object)p2 != null ? new int?(p2.X) : new int?();
            if (!(nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue))
                return false;
            nullable2 = (object)p1 != null ? new int?(p1.Y) : new int?();
            nullable1 = (object)p2 != null ? new int?(p2.Y) : new int?();
            return nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() & nullable2.HasValue == nullable1.HasValue;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            int? nullable1 = (object)p1 != null ? new int?(p1.X) : new int?();
            int? nullable2 = (object)p2 != null ? new int?(p2.X) : new int?();
            if (!(nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue))
                return true;
            nullable2 = (object)p1 != null ? new int?(p1.Y) : new int?();
            nullable1 = (object)p2 != null ? new int?(p2.Y) : new int?();
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
            return string.Format("({0}:{1})", (object)this.X, (object)this.Y);
        }

        protected bool Equals(Point other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if ((object)this == obj)
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return this.Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return this.X * 397 ^ this.Y;
        }
    }
}