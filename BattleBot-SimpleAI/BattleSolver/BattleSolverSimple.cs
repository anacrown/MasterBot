using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using BattleBot_SimpleAI.Controls;
using CodenjoyBot.Annotations;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;
using CodenjoyBot.Interfaces;

namespace BattleBot_SimpleAI.BattleSolver
{
    [Serializable]
    public class BattleSolverSimple : ISolver, INotifyPropertyChanged
    {
        private UIElement _control;
        private UIElement _debugControl;

        public UIElement Control => _control ?? (_control = new BattleSolverControl(this));

        public UIElement DebugControl => _debugControl ?? (_debugControl = new BattleSolverDebugControl(this));

        public BattleSolverSimple()
        {
            
        }

        protected BattleSolverSimple(SerializationInfo info, StreamingContext context) : this()
        {
            
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        public void Initialize()
        {

        }

        public string Answer(string instanceName, DateTime startTime, DataFrame frame)
        {
            throw new NotImplementedException();
        }

        public string Answer(Board<Cell> board)
        {
            OnBoardChanged(board);

            if (board.IsGameOver())
                return string.Empty;

            var map = new Map(board.GetMe()?.Pos, board.GetWeights(), board.Size.Width);
            var enemies = board.GetEnemies().ToArray();
            var min = enemies.Select(t => map[t.Pos]).Min();
            var target = enemies.FirstOrDefault(t => map[t.Pos] == min);

            var path = map.Path(target);
            var where = board.GetMe().Pos.GetDirectionTo(path.First().Pos);
            var command = where.GetCommand() | SolverCommand.Act;

            return command.ToString();
        }

        public event EventHandler<Board<Cell>> BoardChanged;
        public event EventHandler<LogRecord> LogDataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnBoardChanged(Board<Cell> board) => BoardChanged?.Invoke(this, board);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
