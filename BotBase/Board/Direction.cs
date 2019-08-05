using System;

namespace BotBase.Board
{
    public enum Direction
    {
        Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft, Unknown
    }

    public enum RoundDirection
    {
        Clockwise, CounterClockwise
    }

    public static class DirectionExtention
    {
        public static Direction Invert(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Direction.Down;
                case Direction.UpRight: return Direction.DownLeft;
                case Direction.Right: return Direction.Left;
                case Direction.DownRight: return Direction.UpLeft;
                case Direction.Down: return Direction.Up;
                case Direction.DownLeft: return Direction.UpRight;
                case Direction.Left: return Direction.Right;
                case Direction.UpLeft: return Direction.DownRight;
                case Direction.Unknown:
                default: return Direction.Unknown;
            }
        }

        public static Direction Rotate(this Direction direction, RoundDirection roundDirection)
        {
            switch (roundDirection)
            {
                case RoundDirection.Clockwise:
                    return direction.Clockwise();

                case RoundDirection.CounterClockwise:
                    return direction.CounterClockwise();
                default: return Direction.Unknown;
            }
        }

        public static Direction Clockwise(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.UpRight;
                case Direction.UpRight:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.DownRight;
                case Direction.DownRight:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.DownLeft;
                case Direction.DownLeft:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.UpLeft;
                case Direction.UpLeft:
                    return Direction.Up;
                case Direction.Unknown:
                default: return Direction.Unknown;
            }
        }

        public static Direction Clockwise(this Direction direction, int count)
        {
            var d = direction;
            for (var i = 0; i < count; i++) d = d.Clockwise();

            return d;
        }

        public static Direction CounterClockwise(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.UpLeft;
                case Direction.UpRight:
                    return Direction.Up;
                case Direction.Right:
                    return Direction.UpRight;
                case Direction.DownRight:
                    return Direction.Right;
                case Direction.Down:
                    return Direction.DownRight;
                case Direction.DownLeft:
                    return Direction.Down;
                case Direction.Left:
                    return Direction.DownLeft;
                case Direction.UpLeft:
                    return Direction.Left;
                case Direction.Unknown:
                default: return Direction.Unknown;
            }
        }

        //        public static SolverCommand GetCommand(this Direction direction)
        //        {
        //            switch (direction)
        //            {
        //                case Direction.Up: return SolverCommand.Up;
        //                case Direction.Right: return SolverCommand.Right;
        //                case Direction.Down: return SolverCommand.Down;
        //                case Direction.Left: return SolverCommand.Left;
        //                case Direction.Unknown: return SolverCommand.Empty;
        //                default:
        //                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        //            }
        //        }
    }
}