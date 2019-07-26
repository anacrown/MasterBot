using BotBase.Board;

namespace PaperIoStrategy.AISolver
{
    public class SolverCommand
    {
        public static readonly SolverCommand Up = new SolverCommand("up");
        public static readonly SolverCommand Down = new SolverCommand("down");
        public static readonly SolverCommand Left = new SolverCommand("left");
        public static readonly SolverCommand Right = new SolverCommand("right");
        public static readonly SolverCommand Empty = new SolverCommand(string.Empty);
        private readonly string _command;

        public static bool operator ==(SolverCommand cmd1, SolverCommand cmd2)
        {
            return ((object)cmd1 != null ? cmd1._command : null) == ((object)cmd2 != null ? cmd2._command : null);
        }

        public static bool operator !=(SolverCommand cmd1, SolverCommand cmd2)
        {
            return !(cmd1 == cmd2);
        }

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
            return _command.Contains(Left.ToString()) ? Direction.Left : Direction.Unknown;
        }

        public override string ToString()
        {
            return _command;
        }

        protected bool Equals(SolverCommand other)
        {
            return string.Equals(_command, other._command);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((SolverCommand)obj);
        }

        public override int GetHashCode()
        {
            return _command != null ? _command.GetHashCode() : 0;
        }
    }
}