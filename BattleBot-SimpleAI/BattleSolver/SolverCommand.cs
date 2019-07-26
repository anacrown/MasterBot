using System;
using BotBase.Board;

namespace BattleBot_SimpleAI.BattleSolver
{
    public class SolverCommand
    {
        private readonly string _command;
        public static readonly SolverCommand Up = new SolverCommand("UP");
        public static readonly SolverCommand Down = new SolverCommand("DOWN");
        public static readonly SolverCommand Left = new SolverCommand("LEFT");
        public static readonly SolverCommand Right = new SolverCommand("RIGHT");
        public static readonly SolverCommand Act = new SolverCommand("ACT");
        public static readonly SolverCommand Stop = new SolverCommand("STOP");

        public static readonly SolverCommand Empty = new SolverCommand(string.Empty);

        public static SolverCommand operator |(SolverCommand cmd1, SolverCommand cmd2)
        {
            if ((cmd1 != Up || cmd1 != Down || cmd1 != Left || cmd1 != Right) && cmd2 != Act)
                throw new ArgumentException("SolverCommand");

            return new SolverCommand(string.Join(",", cmd1, cmd2));
        }

        public static bool operator ==(SolverCommand cmd1, SolverCommand cmd2) => cmd1?._command == cmd2?._command;
        public static bool operator !=(SolverCommand cmd1, SolverCommand cmd2) => !(cmd1 == cmd2);

        private SolverCommand(string command)
        {
            _command = command;
        }

        public Direction ToDirection()
        {
            if (_command.Contains(Up.ToString()))
                return Direction.Up;

            if (_command.Contains(Right.ToString()))
                return Direction.Right;

            if (_command.Contains(Down.ToString()))
                return Direction.Down;

            if (_command.Contains(Left.ToString()))
                return Direction.Left;

            return Direction.Unknown;
        }

        public override string ToString() => _command == "STOP" ? string.Empty : _command;

        protected bool Equals(SolverCommand other)
        {
            return string.Equals(_command, other._command);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SolverCommand)obj);
        }

        public override int GetHashCode()
        {
            return (_command != null ? _command.GetHashCode() : 0);
        }
    }
}