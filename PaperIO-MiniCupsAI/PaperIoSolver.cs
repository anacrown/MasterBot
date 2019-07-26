using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using BotBase.Interfaces;
using Newtonsoft.Json;
using PaperIO_MiniCupsAI.Controls;
using PaperIoStrategy;
using PaperIoStrategy.AISolver;
using DataFrame = BotBase.BotInstance.DataFrame;
using LogRecord = BotBase.BotInstance.LogRecord;
using Point = System.Windows.Point;

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
            Solver = new PaperIoStrategy.PaperIoSolver();
            Solver.BoardChanged += (sender, board) => OnBoardChanged(board);
        }

        public PaperIoStrategy.PaperIoSolver Solver { get; set; }

        public PaperIoSolver(SerializationInfo info, StreamingContext context) : this() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

        public void Initialize() { }

        public bool Answer(string instanceName, DateTime startTime, DataFrame frame, out string response)
        {
            var b = Solver.Answer(frame.Board, out var rsp);
            response = rsp;
            return b;
        }

        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);
        protected virtual void OnBoardChanged(Board e) => BoardChanged?.Invoke(this, e);
    }
}
