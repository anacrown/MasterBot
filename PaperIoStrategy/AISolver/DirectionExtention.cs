using System;
using BotBase.Board;

namespace PaperIoStrategy.AISolver
{
    public static class DirectionExtention
    {
        public static SolverCommand GetCommand(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return SolverCommand.Up;
                case Direction.Right:
                    return SolverCommand.Right;
                case Direction.Down:
                    return SolverCommand.Down;
                case Direction.Left:
                    return SolverCommand.Left;
                case Direction.Unknown:
                    return SolverCommand.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
        public static Direction ToDirection(this string direction)
        {
            switch (direction)
            {
                case "up":
                    return Direction.Up;
                case "left":
                    return Direction.Left;
                case "right":
                    return Direction.Right;
                case "down":
                    return Direction.Down;
                default:
                    return Direction.Unknown;
            }
        }
    }
}