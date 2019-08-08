using System.Collections.Generic;
using BotBase.Board;
using PaperIoStrategy.AISolver;

namespace PaperIoStrategy
{
    public class BoundPoint
    {
        public bool IsBound { get; set; }
        public bool IsCorner { get; set; }

        public Point Position { get; set; }
        public bool IsTerritory { get; set; }
        public Direction Direction { get; set; }
    }

    public class BBox : Matrix<BoundPoint>
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }

        public BBox(Board board, IEnumerable<Point> territory, Point point1, Point point2) : base(board.Size)
        {
            Point1 = point1;
            Point2 = point2;

            foreach (var point in territory) this[point].IsTerritory = true;

            for (var i = 0; i < Size.Width; i++)
            for (var j = 0; j < Size.Height; j++)
                this[i, j].Position = new Point(i, j);

            UpdateCorners();
        }

        public bool Expand(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:

                    if (!CanExpandUp()) return false;
                    ExpandUp();

                    break;
                case Direction.UpRight:

                    if (!CanExpandUp() || !CanExpandRight()) return false;
                    ExpandUp();
                    ExpandRight();

                    break;
                case Direction.Right:

                    if (!CanExpandRight()) return false;
                    ExpandRight();

                    break;
                case Direction.DownRight:

                    if (!CanExpandDown() || !CanExpandRight()) return false;
                    ExpandDown();
                    ExpandRight();

                    break;
                case Direction.Down:

                    if (!CanExpandDown()) return false;
                    ExpandDown();

                    break;
                case Direction.DownLeft:

                    if (!CanExpandDown() || !CanExpandLeft()) return false;
                    ExpandDown();
                    ExpandLeft();

                    break;
                case Direction.Left:

                    if (!CanExpandLeft()) return false;
                    ExpandLeft();

                    break;
                case Direction.UpLeft:

                    if (!CanExpandUp() || !CanExpandLeft()) return false;
                    ExpandUp();
                    ExpandLeft();

                    break;
                case Direction.Unknown:
                default: return false;
            }

            UpdateCorners();
            return true;
        }

        private bool CanExpandUp() => Point2[Direction.Up].OnBoard(Size);
        private bool CanExpandRight() => Point2[Direction.Right].OnBoard(Size);
        private bool CanExpandDown() => Point1[Direction.Down].OnBoard(Size);
        private bool CanExpandLeft() => Point1[Direction.Left].OnBoard(Size);

        private void ExpandUp() => Point2 = Point2[Direction.Up];
        private void ExpandRight() => Point2 = Point2[Direction.Right];
        private void ExpandDown() => Point1 = Point1[Direction.Down];
        private void ExpandLeft() => Point1 = Point1[Direction.Left];

        private void UpdateCorners()
        {
            foreach (var bound in this)
            {
                bound.IsBound = false;
                bound.IsCorner = false;
            }

            foreach (var point in Point1.GetLine(Direction.Right, Size).While(p => p.X != Point2.X))
            {
                this[point].IsBound = true;
                this[point].Direction = Direction.Left;
            }

            foreach (var point in Point1.GetLine(Direction.Up, Size).While(p => p.Y != Point2.Y))
            {
                this[point].IsBound = true;
                this[point].Direction = Direction.Up;
            }

            foreach (var point in Point2.GetLine(Direction.Left, Size).While(p => p.X != Point1.X))
            {
                this[point].IsBound = true;
                this[point].Direction = Direction.Right;
            }

            foreach (var point in Point2.GetLine(Direction.Down, Size).While(p => p.Y != Point1.Y))
            {
                this[point].IsBound = true;
                this[point].Direction = Direction.Down;
            }

            this[Point1].Direction = Direction.Up;
            this[Point1].IsCorner = this[Point1].IsBound = true;

            this[Point2].Direction = Direction.Down;
            this[Point2].IsCorner = this[Point2].IsBound = true;

            this[Point1.X, Point2.Y].Direction = Direction.Right;
            this[Point1.X, Point2.Y].IsCorner = this[Point1.X, Point2.Y].IsBound = true;

            this[Point2.X, Point1.Y].Direction = Direction.Left;
            this[Point2.X, Point1.Y].IsCorner = this[Point1.X, Point2.Y].IsBound = true;
        }
    }
}