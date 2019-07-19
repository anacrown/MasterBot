using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;
using CodenjoyBot.Interfaces;
using Newtonsoft.Json.Linq;
using PaperIO_MiniCupsAI.Controls;

namespace PaperIO_MiniCupsAI
{
    [Serializable]
    public class PaperIoSolver : ISolver
    {
        public UIElement Control { get; }
        public UIElement DebugControl { get; }

        public event EventHandler<Board> BoardChanged;

        public event EventHandler<LogRecord> LogDataReceived;

        public PaperIoSolver()
        {
            Control = new PaperIoSolverControl(this);
            DebugControl = new PaperIoSolverDebugControl(this);
        }

        public PaperIoSolver(SerializationInfo info, StreamingContext context) : this() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

        public void Initialize() { }
        public string Answer(Board board)
        {
            LoadData(board);

            OnBoardChanged(board);

            var commands = new string[4] { "left", "right", "up", "down" };
            var random = new Random();
            while (true)
            {
                var index = random.Next(0, commands.Length);
                return $"{{\"command\": \"{commands[index]}\"}}";
            }
        }

        private void LoadData(Board board)
        {
            var jObject = JObject.Parse(board.Frame.Board);

            var type = jObject["type"];
        }

        protected virtual void OnBoardChanged(Board board) => BoardChanged?.Invoke(this, board);
    }
}
