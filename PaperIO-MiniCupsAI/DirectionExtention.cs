﻿using System;
using CodenjoyBot.Board;

namespace PaperIO_MiniCupsAI
{
    public static class DirectionExtention
    {
        public static Direction Invert(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Unknown:
                    return Direction.Unknown;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
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